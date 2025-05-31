import React from "react";
import test from "node:test";
import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";

import { db } from "../lib/db";
import { Rack } from "@prisma/client";
import { getAllRacks, addNewRack, deleteRackById, editRackById } from "../data/local/rackRepo";

jest.mock("../lib/db", () => ({
    db: {
        rack: {
            findMany: jest.fn(),
            create: jest.fn(),
            update: jest.fn(),
            delete: jest.fn(),
        }
    }
}));


describe("getAllRacks", () => {
    it("should retrieve all racks from the database", async () => {
        const racks: Rack[] = [
            { id: 1, name: "Rack 1", location: "Location 1" },
            { id: 2, name: "Rack 2", location: "Location 2" }
        ];

        (db.rack.findMany as jest.Mock).mockReturnValueOnce(racks);

        const result = await getAllRacks();

        expect(db.rack.findMany).toHaveBeenCalled();
        expect(result).toEqual(racks);
    });
});


describe("addNewRack", () => {
    it("should create a new rack in the database", async () => {
        const data = { name: "New Rack", location: "New Location" };
        const newRack = { id: 1, name: data.name, location: data.location };

        (db.rack.create as jest.Mock).mockReturnValueOnce(newRack);

        const result = await addNewRack(data);

        expect(db.rack.create).toHaveBeenCalledWith({
            data: {
                name: data.name,
                location: data.location
            }
        });

        expect(result).toEqual(newRack);
    });
});

describe("deleteRackById", () => {
    it("should delete a rack from the database by its ID", async () => {
        const rackId = 1;

        await deleteRackById(rackId);

        expect(db.rack.delete).toHaveBeenCalledWith({
            where: { id: rackId }
        });
    });
});

describe("editRackById", () => {
    it("should edit the location and name of a rack by its ID", async () => {
        const rackId = 1;
        const location = "Updated Location";
        const name = "Updated Name";

        await editRackById(rackId, location, name);

        expect(db.rack.update).toHaveBeenCalledWith({
            where: { id: rackId },
            data: {
                location: location,
                name: name
            }
        });
    });
});