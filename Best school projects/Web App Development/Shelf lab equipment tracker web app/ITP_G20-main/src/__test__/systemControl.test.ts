import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { retrieveSystemStatus, logStatusChange, getHostIpConfig } from "../domain/systemControl";
import { getLatestSystemStatus, addSystemStatus } from "../data/local/systemStatusRepo";
import { SystemStatus } from "@prisma/client";
import Conf from "conf";

jest.mock('conf', () => {
    return jest.fn().mockImplementation(() => {
        return {
            get: jest.fn(),
        };
    });
});

jest.mock("../data/local/systemStatusRepo", () => ({
    getLatestSystemStatus: jest.fn(),
    addSystemStatus: jest.fn()
}));

describe("retrieveSystemStatus", () => {
    it("should retrieve the latest system status", async () => {
        const latestStatus: SystemStatus = { id: "1", systemStatus: true, systemTime: new Date() };

        (getLatestSystemStatus as jest.Mock).mockReturnValueOnce(latestStatus);

        const result = await retrieveSystemStatus();

        expect(getLatestSystemStatus).toHaveBeenCalled();
        expect(result).toEqual(latestStatus);
    });

    it("should log status change if no latest status is found", async () => {
        (getLatestSystemStatus as jest.Mock).mockReturnValueOnce(null);
        const statusChange: SystemStatus = { id: "2", systemStatus: false, systemTime: new Date() };

        (addSystemStatus as jest.Mock).mockReturnValueOnce(statusChange);

        const result = await retrieveSystemStatus();

        expect(getLatestSystemStatus).toHaveBeenCalled();
        expect(addSystemStatus).toHaveBeenCalledWith({ systemStatus: false, systemTime: expect.any(Date) });
        expect(result).toEqual(statusChange);
    });
});

describe("logStatusChange", () => {
    it("should log the status change of the system", async () => {
        const statusChange: SystemStatus = { id: "1", systemStatus: true, systemTime: new Date() };

        (addSystemStatus as jest.Mock).mockReturnValueOnce(statusChange);

        const result = await logStatusChange(true);

        expect(addSystemStatus).toHaveBeenCalledWith({ systemStatus: true, systemTime: expect.any(Date) });
        expect(result).toEqual(statusChange);
    });
});

