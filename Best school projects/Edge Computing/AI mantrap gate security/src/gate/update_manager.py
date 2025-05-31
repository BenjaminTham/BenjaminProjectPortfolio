from queue import Queue

from network.api.client import UpdateDownloader
from utils.logger_mixin import LoggerMixin


class UpdateManager(LoggerMixin):
    """
    This class manages update operations, including handling updates in a controlled way,
    supporting queuing when the system is busy, and downloading updates in parallel. It
    orchestrates various aspects of update handling such as logging, managing the update
    queue, and leveraging multithreading for download tasks.

    The class works in conjunction with a gate object to assess system readiness and an
    external update configuration to specify update parameters like source URL and save
    directory. It employs an UpdateDownloader instance to perform actual download tasks
    and allows callbacks to handle post-download processing and error reporting.

    :ivar gate: A gate object to control the update process based on system readiness.
    :type gate: Gate
    :ivar update_url: Base URL for downloading the updates.
    :type update_url: str
    :ivar update_save_dir: Local directory to save downloaded updates.
    :type update_save_dir: str
    :ivar update_thread: Currently active update thread, if any.
    :type update_thread: Optional[UpdateDownloader]
    :ivar update_queue: A queue to store pending updates when the system is busy.
    :type update_queue: Queue
    :ivar logger: Logger instance for tracking update operations.
    :type logger: Logger
    """

    def __init__(self, gate, update_config):
        """
        Represents a component responsible for managing updates within an application.
        This class initializes and maintains configurations for updates, including URLs
        and directories for saving updates. It also sets up logging facilities and
        provides mechanisms to enqueue update-related tasks.

        :param gate: The gate object responsible for application-level functionality,
                     including logging and communication.
        :param update_config: A dictionary containing update configuration parameters.
                              Expected keys are:
                              "url" - The URL to fetch update data from.
                              "save_path" - The directory path where updates are saved.

        :type gate: Any
        :type update_config: dict
        """
        self.gate = gate
        self.update_url = update_config["url"]
        self.update_save_dir = update_config["save_path"]
        self.update_thread = None
        self.update_queue = Queue()
        self.logger = self.setup_logger(__name__, self.gate.logger.level)

    def handle_update(self, updates):
        """
        Handles the processing of update requests and manages the download and application
        of updates. This method ensures updates are either queued or appropriately executed
        based on the system's current state. It utilizes a separate thread to download and
        process updates and provides success or failure feedback accordingly.

        :param updates: A dictionary with update types as keys and corresponding update filenames as values.
        :type updates: dict[str, str]
        :return: A boolean indicating whether the update handling process was initiated successfully.
        :rtype: bool
        """
        if self.gate.is_busy:
            self.logger.info("System is busy, queueing updates")
            self.update_queue.put(updates)
            return True

        if self.update_thread and self.update_thread.is_alive():
            self.gate.logger.warning("Update already in progress")
            return False

        # Create URLs dictionary
        download_urls = {
            update_type: f"{self.update_url}{filename}"
            for update_type, filename in updates.items()
        }

        def update_callback(success, results=None):
            if success:
                self.gate.logger.info("All updates completed successfully")
            else:
                self.gate.logger.error("Some updates failed")
                if results:
                    for update_type, result in results.items():
                        self.gate.logger.info(
                            f"{update_type} update: {'Success' if result else 'Failed'}"
                        )
            self.update_thread = None

        self.update_thread = UpdateDownloader(
            download_urls,
            self.update_save_dir,
            update_callback,
            self.gate.logger.level,
        )
        self.update_thread.start()
        return True

    def process_pending_updates(self):
        """
        Processes pending updates by retrieving them from the update queue and
        handling them if the gate is not currently busy. This function ensures
        that queued updates are processed in a controlled manner.

        :raises queue.Empty: If the queue is empty when attempting to retrieve
            updates.
        """
        if not self.update_queue.empty() and not self.gate.is_busy:
            updates = self.update_queue.get()
            self.gate.logger.info("Processing queued updates")
            self.handle_update(updates)
