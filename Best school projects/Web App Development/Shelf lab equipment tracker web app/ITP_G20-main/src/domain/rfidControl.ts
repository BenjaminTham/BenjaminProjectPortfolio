"use server";

import {
    startInventoryRequest,
    stopInventoryRequest,
    tagReadRequest,
    tagReportingDataAndIndex,
    tagWriteRequest,
    updateAntennaEnabledState,
    updateAntennaPower,
    updateMqttConfigurationRequest
} from "@/data/remote/ura8";
import {getHostIpConfig, getPowerLevelConfig, logStatusChange, retrieveSystemStatus} from "@/domain/systemControl";
import {retrieveAntennaByFunction} from "@/domain/readerControl";
import {getAllReaders} from "@/data/local/readerRepo";
import isPortReachable from "is-port-reachable";
import { editTray, retrieveAllTray } from "@/domain/trayControl";
import { timeOut } from "@/domain/mqttControl";


/**
 * Run RFID reporting data for all readers.
 *
 * @returns {Promise<void>} A promise that resolves when all the RFID reporting data has been fetched.
 */
export async function runRFIDReportingData(): Promise<void> {
    const systemStatus = await retrieveSystemStatus();
    if (!systemStatus?.systemStatus) {
        return;
    }
    const readers = await getAllReaders();

    const trays = await retrieveAllTray();
    const currentTime = new Date();
    const lastMessageTime = global.lastMessageTime;
    await Promise.all(trays.map(async tray => { 
        if (tray.status === "OUT_OF_BOUND") {
            return;
        }
        if (tray.status === "IN" && lastMessageTime && (currentTime.getTime() - lastMessageTime.getTime()) > timeOut)  {
            tray.status = "OUT";
            console.log(`Tray ${tray.id} is out at ${new Date()} with rackId ${tray.rackId || undefined} for ${currentTime.getTime() - lastMessageTime.getTime()}`)
            await editTray(tray.id, tray.epc, tray.status, tray.statusChg, tray.rackId as number || undefined);
        }
    }));

    for (const reader of readers) {
        tagReportingDataAndIndex(reader.ip);
    }
}

/**
 * Starts the inventory process for RFID readers.
 * If the system status is not running, it sets the system status to running,
 * retrieves all readers, and starts the inventory request for each reader.
 * Finally, it logs the status change.
 */
export async function startInventory() {
    const systemStatus = await retrieveSystemStatus();
    if (!systemStatus?.systemStatus) {

        // Check if the host IP is configured
        const hostIp = await getHostIpConfig();
        if (!hostIp) {
            return {error: "Host IP is not configured! Please configure the IP address in the config."};
        }

        // Check if the host IP is reachable
        const ipPortReachable = await isPortReachable(1883, {host: hostIp});

        if (!ipPortReachable) {
            return {error: "Host IP is not reachable! Please ensure the IP address is correct."};
        }

        const readers = await getAllReaders();

        // If no readers are found, return an error
        if (readers.length === 0) {
            return {error: "No readers found! Please add at least one reader."};
        }

        if (process.env.MQTT_PASSWORD === undefined) {
            return {error: "MQTT password is not set! Please set the password in the .env file."};
        }

        const powerLevelConfig = await getPowerLevelConfig();
        if (!powerLevelConfig) {
            return {error: "Power level is not configured! Please configure the power level in the config."};
        }

        // Check if the IP address of the readers is reachable
        for (const reader of readers) {
            const ipPortReachable = await isPortReachable(8080, {host: reader.ip});
            if (!ipPortReachable) {
                return {error: `Reader IP ${reader.ip} is not reachable! Please check the connection.`};
            }
            else {
                // Update the reader MQTT settings
                const updateMqttConfigurationResponse = await updateMqttConfigurationRequest(reader.ip, true, hostIp, "reader", reader.ip, "admin", process.env.MQTT_PASSWORD, "1883");
                if (updateMqttConfigurationResponse.normalConfiguration.code !== 0) {
                    return {error: `Unable to update MQTT configuration for reader ${reader.ip} due to error: ${updateMqttConfigurationResponse.message}. Please try again.`};
                }
            }
        }

        const { inventoryPower, geofencingPower } = powerLevelConfig;

        for (const reader of readers) {
            // Find all antennas with the INVENTORY or GEOFENCE function and set their enabled state to true
            const antennas = reader.antennas.filter(antenna => antenna.function === "INVENTORY" || antenna.function === "GEOFENCE");
            const antennaPorts = antennas.map(antenna => antenna.antennaPort);
            console.log(antennaPorts);

            // Enable antennas
            const antennasEnable = Array.from({ length: 8 }, (_, i) => ({
                antennaPort: i + 1,
                enable: antennaPorts.includes(i + 1),
            }));

            // Set power levels for antennas based on their function
            const antennasPower = Array.from({ length: 8 }, (_, i) => {
                const antenna = antennas.find(antenna => antenna.antennaPort === (i + 1));
                let power = 1;
                if (antenna) {
                    power = antenna.function === "INVENTORY" ? inventoryPower : geofencingPower;
                }
                return {
                    antennaPort: i + 1,
                    power: power,
                };
            });

            console.log(antennasEnable);
            console.log(antennasPower);
            await updateAntennaEnabledState(reader.ip, antennasEnable);
            await updateAntennaPower(reader.ip, antennasPower);
            await startInventoryRequest(reader.ip)
        }
        await logStatusChange(true);
    }
}

/**
 * Stops the inventory process by setting the system status to not running.
 * If the system status is running, it retrieves all readers and sends a start inventory request to each reader's IP address.
 * Finally, it logs the status change to false.
 */
export async function stopInventory() {
    const systemStatus = await retrieveSystemStatus();
    if (systemStatus?.systemStatus) {
        await logStatusChange(false);
        const readers = await getAllReaders();
        console.log("Stop inventory")
        for (const reader of readers) {
            await stopInventoryRequest(reader.ip)
        }
    }
}

export async function updateAntennaPowerLevel(ip: string, powerLevel: number) {
    const antennas = Array.from({length: 8}, (_, i) => ({
        antennaPort: i + 1,
        power: powerLevel
    }));
    await updateAntennaPower(ip, antennas);
}

async function enableReadWriteAntennas(readWriteAntennas: { readerIp: string, antennaPort: number }[]) {
    // Initialize an array of antennas with their enabled state set to false
    const antennas = Array.from({length: 8}, (_, index) => ({
        antennaPort: index + 1,
        enable: false,
    }));

    // If a READWRITE antenna is found, set its enabled state to true
    if (readWriteAntennas.length > 0) {
        const readWriteAntenna = readWriteAntennas[0];
        antennas[readWriteAntenna.antennaPort - 1].enable = true;
    } else {
        return {error: "No READWRITE antenna found"};
    }

    // Enable the READWRITE antenna
    await updateAntennaEnabledState(readWriteAntennas[0].readerIp, antennas);

    // Set power
    const powerLevelConfig = await getPowerLevelConfig();
    if (!powerLevelConfig) {
        return {error: "Power level is not configured! Please configure the power level in the config."};
    }
    await updateAntennaPowerLevel(readWriteAntennas[0].readerIp, powerLevelConfig.readwritePower);
}

/**
 * Reads an RFID tag using the READWRITE antenna.
 * @returns A promise that resolves to an object with an error message if no READWRITE antenna is found, or void if successful.
 */
export async function readRfidTag(): Promise<{ error: string } | string> {
    // If the system is still running, return an error
    const systemStatus = await retrieveSystemStatus();

    if (systemStatus?.systemStatus) {
        return {error: "Inventory is running. Please stop the system before reading RFID tags."};
    }

    // Retrieve the antennas with the READWRITE function
    const readWriteAntennas = await retrieveAntennaByFunction("READWRITE");
    console.log(readWriteAntennas)

    // Check if the reader port is reachable
    const ipPortReachable = await isPortReachable(8080, {host: readWriteAntennas[0].readerIp});

    if (!ipPortReachable) {
        return {error: `Reader IP ${readWriteAntennas[0].readerIp} is not reachable! Please check the connection.`};
    }

    // Enable the READWRITE antenna
    const enableReadWriteAntennasResponse = await enableReadWriteAntennas(readWriteAntennas);
    if (enableReadWriteAntennasResponse?.error) {
        return {error: enableReadWriteAntennasResponse.error};
    }

    // Start the tag read request
    const response = await tagReadRequest(readWriteAntennas[0].readerIp);

    if (response.code !== 0) {
        return {error: `Unable to read antenna due to error: ${response.message}. Please try again.`};
    }

    return response.hexTagData
}

export async function writeRfidTag(originalEpc: string, newEpc: string): Promise<{ error: string } | void> {
    // If the system is still running, return an error
    const systemStatus = await retrieveSystemStatus();

    if (systemStatus?.systemStatus) {
        return {error: "Inventory is running. Please stop the system before writing RFID tags."};
    }

    // Retrieve the antennas with the READWRITE function
    const readWriteAntennas = await retrieveAntennaByFunction("READWRITE");

    // Check if the reader port is reachable
    const ipPortReachable = await isPortReachable(8080, {host: readWriteAntennas[0].readerIp});

    if (!ipPortReachable) {
        return {error: `Reader IP ${readWriteAntennas[0].readerIp} is not reachable! Please check the connection.`};
    }

    // Enable the READWRITE antenna
    const enableReadWriteAntennasResponse = await enableReadWriteAntennas(readWriteAntennas);
    if (enableReadWriteAntennasResponse?.error) {
        return {error: enableReadWriteAntennasResponse.error};
    }

    // Start the tag read request
    const tagWriteResponse = await tagWriteRequest(
        readWriteAntennas[0].readerIp,
        newEpc,
        undefined,
        undefined,
        undefined,
        undefined,
        true,
        undefined,
        undefined,
        undefined,
        originalEpc
    );
    
    if (tagWriteResponse.code !== 0) {
        return {error: `Unable to write tag due to error: ${tagWriteResponse.message}. Please try again.`};
    }
}
