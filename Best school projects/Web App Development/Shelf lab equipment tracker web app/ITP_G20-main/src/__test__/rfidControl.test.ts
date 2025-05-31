import React from "react";
import test from "node:test";
import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";

import {
    readRfidTag,
    runRFIDReportingData,
    startInventory,
    stopInventory,
    writeRfidTag
} from "../domain/rfidControl";
import {
    retrieveSystemStatus,
    logStatusChange,
    getHostIpConfig,
    getPowerLevelConfig
} from "../domain/systemControl";
import { retrieveAntennaByFunction } from "../domain/readerControl";
import { getAllReaders } from "../data/local/readerRepo";
import { retrieveAllTray, editTray } from "../domain/trayControl";
import {
    startInventoryRequest,
    stopInventoryRequest,
    tagReadRequest,
    tagReportingDataAndIndex,
    tagWriteRequest,
    updateAntennaEnabledState,
    updateAntennaPower,
    updateMqttConfigurationRequest
} from "../data/remote/ura8";
import isPortReachable from "is-port-reachable";

jest.mock("../domain/systemControl", () => ({
    retrieveSystemStatus: jest.fn(),
    logStatusChange: jest.fn(),
    getHostIpConfig: jest.fn(),
    getPowerLevelConfig: jest.fn(),
}));

jest.mock("../data/local/readerRepo", () => ({
    getAllReaders: jest.fn(),
}));

jest.mock("../domain/trayControl", () => ({
    retrieveAllTray: jest.fn(),
    editTray: jest.fn()
}));

jest.mock("../data/remote/ura8", () => ({
    tagReportingDataAndIndex: jest.fn(),
    updateAntennaEnabledState: jest.fn(),
    updateAntennaPower: jest.fn(),
    startInventoryRequest: jest.fn(),
    updateMqttConfigurationRequest: jest.fn(),
    stopInventoryRequest: jest.fn(),
    tagReadRequest: jest.fn(),
    tagWriteRequest: jest.fn()
}));

jest.mock("../domain/readerControl", () => ({
    retrieveAntennaByFunction: jest.fn()
}));

jest.mock("is-port-reachable");

describe("runRFIDReportingData", () => {
    it("should run RFID reporting data for all readers", async () => {
        const systemStatus = { systemStatus: true };
        (retrieveSystemStatus as jest.Mock).mockReturnValueOnce(systemStatus);

        const readers = [{ ip: "192.168.1.1" }];
        (getAllReaders as jest.Mock).mockReturnValueOnce(readers);

        const trays = [{ id: 1, status: "IN", epc: "EPC1", statusChg: new Date(), rackId: 1 }];
        (retrieveAllTray as jest.Mock).mockReturnValueOnce(trays);

        global.lastMessageTime = new Date();

        await runRFIDReportingData();

        expect(retrieveSystemStatus).toHaveBeenCalled();
        expect(getAllReaders).toHaveBeenCalled();
        expect(retrieveAllTray).toHaveBeenCalled();
        expect(editTray).toHaveBeenCalled();
        expect(tagReportingDataAndIndex).toHaveBeenCalledWith(readers[0].ip);
    });
});

describe("startInventory", () => {
    it("should start the inventory process for RFID readers", async () => {
        const systemStatus = { systemStatus: false };
        (retrieveSystemStatus as jest.Mock).mockReturnValueOnce(systemStatus);

        const hostIp = "192.168.1.1";
        (getHostIpConfig as jest.Mock).mockReturnValueOnce(hostIp);

        (isPortReachable as jest.Mock).mockReturnValueOnce(true);

        const readers = [{ ip: "192.168.1.2", antennas: [{ antennaPort: 1, function: "INVENTORY" }] }];
        (getAllReaders as jest.Mock).mockReturnValueOnce(readers);

        process.env.MQTT_PASSWORD = "password";

        const powerLevelConfig = { inventoryPower: 30, geofencingPower: 20 };
        (getPowerLevelConfig as jest.Mock).mockReturnValueOnce(powerLevelConfig);

        await startInventory();

        expect(retrieveSystemStatus).toHaveBeenCalled();
        expect(getHostIpConfig).toHaveBeenCalled();
        expect(isPortReachable).toHaveBeenCalledWith(1883, { host: hostIp });
        expect(getAllReaders).toHaveBeenCalled();
        expect(getPowerLevelConfig).toHaveBeenCalled();
        expect(updateMqttConfigurationRequest).toHaveBeenCalled();
        expect(updateAntennaEnabledState).toHaveBeenCalled();
        expect(updateAntennaPower).toHaveBeenCalled();
        expect(startInventoryRequest).toHaveBeenCalled();
        expect(logStatusChange).toHaveBeenCalledWith(true);
    });
});

describe("stopInventory", () => {
    it("should stop the inventory process for RFID readers", async () => {
        const systemStatus = { systemStatus: true };
        (retrieveSystemStatus as jest.Mock).mockReturnValueOnce(systemStatus);

        const readers = [{ ip: "192.168.1.2" }];
        (getAllReaders as jest.Mock).mockReturnValueOnce(readers);

        await stopInventory();

        expect(retrieveSystemStatus).toHaveBeenCalled();
        expect(logStatusChange).toHaveBeenCalledWith(false);
        expect(getAllReaders).toHaveBeenCalled();
        expect(stopInventoryRequest).toHaveBeenCalledWith(readers[0].ip);
    });
});

describe("readRfidTag", () => {
    it("should read an RFID tag using the READWRITE antenna", async () => {
        const systemStatus = { systemStatus: false };
        (retrieveSystemStatus as jest.Mock).mockReturnValueOnce(systemStatus);

        const readWriteAntennas = [{ readerIp: "192.168.1.2", antennaPort: 1 }];
        (retrieveAntennaByFunction as jest.Mock).mockReturnValueOnce(readWriteAntennas);

        (isPortReachable as jest.Mock).mockReturnValueOnce(true);

        const tagReadResponse = { code: 0, hexTagData: "EPC123" };
        (tagReadRequest as jest.Mock).mockReturnValueOnce(tagReadResponse);

        const result = await readRfidTag();

        expect(retrieveSystemStatus).toHaveBeenCalled();
        expect(retrieveAntennaByFunction).toHaveBeenCalledWith("READWRITE");
        expect(isPortReachable).toHaveBeenCalledWith(8080, { host: readWriteAntennas[0].readerIp });
        expect(tagReadRequest).toHaveBeenCalledWith(readWriteAntennas[0].readerIp);
        expect(result).toEqual(tagReadResponse.hexTagData);
    });

    it("should return an error if inventory is running", async () => {
        const systemStatus = { systemStatus: true };
        (retrieveSystemStatus as jest.Mock).mockReturnValueOnce(systemStatus);

        const result = await readRfidTag();

        expect(retrieveSystemStatus).toHaveBeenCalled();
        expect(result).toEqual({ error: "Inventory is running. Please stop the system before reading RFID tags." });
    });
});

describe("writeRfidTag", () => {
    it("should write an RFID tag using the READWRITE antenna", async () => {
        const systemStatus = { systemStatus: false };
        (retrieveSystemStatus as jest.Mock).mockReturnValueOnce(systemStatus);

        const readWriteAntennas = [{ readerIp: "192.168.1.2", antennaPort: 1 }];
        (retrieveAntennaByFunction as jest.Mock).mockReturnValueOnce(readWriteAntennas);

        (isPortReachable as jest.Mock).mockReturnValueOnce(true);

        const tagWriteResponse = { code: 0 };
        (tagWriteRequest as jest.Mock).mockReturnValueOnce(tagWriteResponse);

        const result = await writeRfidTag("originalEpc", "newEpc");

        expect(retrieveSystemStatus).toHaveBeenCalled();
        expect(retrieveAntennaByFunction).toHaveBeenCalledWith("READWRITE");
        expect(isPortReachable).toHaveBeenCalledWith(8080, { host: readWriteAntennas[0].readerIp });
        expect(tagWriteRequest).toHaveBeenCalledWith(readWriteAntennas[0].readerIp, "newEpc", undefined, undefined, undefined, undefined, true, undefined, undefined, undefined, "originalEpc");
        expect(result).toBeUndefined();
    });

    it("should return an error if inventory is running", async () => {
        const systemStatus = { systemStatus: true };
        (retrieveSystemStatus as jest.Mock).mockReturnValueOnce(systemStatus);

        const result = await writeRfidTag("originalEpc", "newEpc");

        expect(retrieveSystemStatus).toHaveBeenCalled();
        expect(result).toEqual({ error: "Inventory is running. Please stop the system before writing RFID tags." });
    });
});
