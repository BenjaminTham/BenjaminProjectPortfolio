import React from "react";
import test from "node:test";
import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";

import { addAntenna, getAllGeofenceAntennas, updateAntenna } from "../data/local/antennaRepo";
import { db } from "../lib/db";
import { Antenna, Function } from "@prisma/client";

// Mock the Prisma client
jest.mock("../lib/db", () => ({
    db: {
        antenna: {
            create: jest.fn(),
            findMany: jest.fn(),
            update: jest.fn()
        }
    }
}));

describe("addAntenna", () => {
    it("should add an antenna to the database", async () => {
        const data = {
            antennaPort: 1,
            function: Function.INVENTORY
        };

        const expectedAntenna = {
            id: "some-unique-id",
            antennaPort: data.antennaPort,
            function: data.function,
            readerId: null
        };

        (db.antenna.create as jest.Mock).mockReturnValueOnce(expectedAntenna);

        const result = await addAntenna(data);

        expect(db.antenna.create).toHaveBeenCalledWith({
            data: {
                antennaPort: data.antennaPort,
                function: data.function
            }
        });

        expect(result).toEqual(expectedAntenna);
    });
});

describe("getAllGeofenceAntennas", () => {
    it("should retrieve all antennas with the GEOFENCE function", async () => {
        const geofenceAntennas: Antenna[] = [
            {
                id: "antenna-1",
                antennaPort: 1,
                function: Function.GEOFENCE,
                readerId: null
            },
            {
                id: "antenna-2",
                antennaPort: 2,
                function: Function.GEOFENCE,
                readerId: null
            }
        ];

        (db.antenna.findMany as jest.Mock).mockReturnValueOnce(geofenceAntennas);

        const result = await getAllGeofenceAntennas();

        expect(db.antenna.findMany).toHaveBeenCalledWith({
            where: {
                function: Function.GEOFENCE
            }
        });

        expect(result).toEqual(geofenceAntennas);
    });
});

describe("updateAntenna", () => {
    it("should update an antenna in the database", async () => {
        const data = {
            id: "antenna-1",
            antennaPort: 1,
            function: Function.READWRITE
        };

        const updatedAntenna = {
            id: data.id,
            antennaPort: data.antennaPort,
            function: data.function,
            readerId: null
        };

        // Mock the response of db.antenna.update
        (db.antenna.update as jest.Mock).mockReturnValueOnce(updatedAntenna);

        const result = await updateAntenna(data);

        // Check if db.antenna.update was called with the correct parameters
        expect(db.antenna.update).toHaveBeenCalledWith({
            where: {
                id: data.id
            },
            data: {
                antennaPort: data.antennaPort,
                function: data.function
            }
        });

        // Check if the result is as expected
        expect(result).toEqual(updatedAntenna);
    });
});