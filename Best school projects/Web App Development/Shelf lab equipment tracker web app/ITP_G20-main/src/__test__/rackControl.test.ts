import React from "react";
import test from "node:test";
import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";


import { createNewRack, deleteRack, retrieveAllRacks, updateRack } from "../domain/rackControl";
import { addNewRack, deleteRackById, editRackById, getAllRacks } from "../data/local/rackRepo";
import { Rack } from "@prisma/client";

jest.mock("../data/local/rackRepo", () => ({
    getAllRacks: jest.fn(),
    addNewRack: jest.fn(),
    editRackById: jest.fn(),
    deleteRackById: jest.fn()
}));

describe("retrieveAllRacks", () => {
    it("should retrieve all racks from the database", async () => {
        const racks: Rack[] = [
            { id: 1, location: "Location1", name: "Rack1" },
            { id: 2, location: "Location2", name: "Rack2" }
        ];

        (getAllRacks as jest.Mock).mockReturnValueOnce(racks);

        const result = await retrieveAllRacks();

        expect(getAllRacks).toHaveBeenCalled();
        expect(result).toEqual(racks);
    });
});

describe("createNewRack", () => {
    it("should create a new rack with valid values", async () => {
        const values = { location: "Location1", name: "Rack1" };
        const rack = { id: 1, location: values.location, name: values.name };

        (addNewRack as jest.Mock).mockReturnValueOnce(rack);

        const result = await createNewRack(values);

        expect(addNewRack).toHaveBeenCalledWith(values);
        expect(result).toEqual(rack);
    });

    it("should throw an error with invalid values", async () => {
        const values = { location: "", name: "Rack1" };

        await expect(createNewRack(values)).rejects.toThrow("Invalid input");
    });
});

describe("updateRack", () => {
    it("should update a rack with valid values", async () => {
        const values = { rackId: 1, location: "New Location", name: "New Name" };

        await updateRack(values);

        expect(editRackById).toHaveBeenCalledWith(values.rackId, values.location, values.name);
    });

    it("should throw an error with invalid values", async () => {
        const values = { rackId: 0, location: "New Location", name: "New Name" };

        await expect(updateRack(values)).rejects.toThrow("Invalid input");
    });
});


describe("deleteRack", () => {
    beforeEach(() => {
        jest.spyOn(console, 'log').mockImplementation(() => { });
        jest.spyOn(console, 'error').mockImplementation(() => { });
    });

    it("should delete a rack by its ID", async () => {
        const rackId = 1;

        await deleteRack(rackId);

        expect(deleteRackById).toHaveBeenCalledWith(rackId);
        expect(console.log).toHaveBeenCalledWith(`Rack with ID ${rackId} has been deleted.`);
    });

    it("should throw an error if unable to delete the rack", async () => {
        const rackId = 1;
        const error = new Error("Error deleting rack");

        (deleteRackById as jest.Mock).mockRejectedValueOnce(error as never);

        await expect(deleteRack(rackId)).rejects.toThrow(`Unable to delete rack with ID ${rackId}`);
        expect(console.error).toHaveBeenCalledWith(`Error deleting rack with ID ${rackId}:`, error);
    });
});