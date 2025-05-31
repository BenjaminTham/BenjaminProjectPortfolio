import React from "react";
import test from "node:test";
import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";

import { addSystemStatus, getLatestSystemStatus } from "../data/local/systemStatusRepo";
import { db } from "../lib/db";
import { SystemStatus } from "@prisma/client";

jest.mock("../lib/db", () => ({
    db: {
        systemStatus: {
            create: jest.fn(),
            findFirst: jest.fn(),

        }
    }
}));

describe("addSystemStatus", () => {
    it("should add a new system status record to the database", async () => {
        const data = { systemStatus: true, systemTime: new Date() };
        const newSystemStatus: SystemStatus = { id: "1", systemStatus: data.systemStatus, systemTime: data.systemTime };

        (db.systemStatus.create as jest.Mock).mockReturnValueOnce(newSystemStatus);

        const result = await addSystemStatus(data);

        expect(db.systemStatus.create).toHaveBeenCalledWith({
            data: {
                systemStatus: data.systemStatus,
                systemTime: data.systemTime,
            },
        });
        expect(result).toEqual(newSystemStatus);
    });
});

describe("getLatestSystemStatus", () => {
    it("should retrieve the latest system status from the database", async () => {
        const latestSystemStatus: SystemStatus = { id: "1", systemStatus: true, systemTime: new Date() };

        (db.systemStatus.findFirst as jest.Mock).mockReturnValueOnce(latestSystemStatus);

        const result = await getLatestSystemStatus();

        expect(db.systemStatus.findFirst).toHaveBeenCalledWith({
            orderBy: {
                systemTime: "desc",
            },
        });
        expect(result).toEqual(latestSystemStatus);
    });

    it("should return null if no system status is found", async () => {
        (db.systemStatus.findFirst as jest.Mock).mockReturnValueOnce(null);

        const result = await getLatestSystemStatus();

        expect(db.systemStatus.findFirst).toHaveBeenCalledWith({
            orderBy: {
                systemTime: "desc",
            },
        });
        expect(result).toBeNull();
    });
});