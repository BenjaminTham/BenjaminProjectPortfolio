import logging


class LoggerMixin:
    """
    Provides a mixin for adding logging capabilities to classes.

    The LoggerMixin class provides a static utility method for setting up
    a logger with a consistent format and optional configuration for
    custom logging levels. This mixin can be used to facilitate logging
    across various components of an application.

    """

    @staticmethod
    def setup_logger(name, level=logging.INFO):
        """
        Configures and returns a logger instance with a specified name and logging level.
        If the logger does not already have handlers, a console handler is added, formatted,
        and attached. This setup ensures that logged messages adhere to the specified
        logging format and level.

        :param name: The name of the logger being configured.
        :param level: The logging level for the logger (e.g., logging.INFO, logging.DEBUG).
        :return: A configured logger instance.
        :rtype: logging.Logger
        """
        logger = logging.getLogger(name)
        logger.setLevel(level)

        # Add console handler if none exists
        if not logger.handlers:
            console_handler = logging.StreamHandler()
            console_handler.setLevel(level)
            formatter = logging.Formatter(
                "%(asctime)s - %(name)s - %(levelname)s - %(message)s"
            )
            console_handler.setFormatter(formatter)
            logger.addHandler(console_handler)
            logger.propagate = False

        return logger
