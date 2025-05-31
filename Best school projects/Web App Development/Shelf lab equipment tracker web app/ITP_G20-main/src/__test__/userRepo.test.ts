import React from "react";
import test from "node:test";
import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";

import {
    getUserById,
    getUserByEmail,
    createUser,
    updateUser,
    getNumberOfUsers
} from "../data/local/userRepo";
import { db } from "../lib/db";
import { User } from "@prisma/client";

jest.mock("../lib/db", () => ({
    db: {
        user: {
            findUnique: jest.fn(),
            create: jest.fn(),
            update: jest.fn(),
            count: jest.fn(),
        }
    }
}));


describe("getNumberOfUsers", () => {
    it("should retrieve the number of users from the database", async () => {
        (db.user.count as jest.Mock).mockReturnValueOnce(42);

        const result = await getNumberOfUsers();

        expect(db.user.count).toHaveBeenCalled();
        expect(result).toEqual(42);
    });
});

describe("updateUser", () => {
    it("should update a user with the provided data", async () => {
        const id = "1";
        const data = { email: "updated@example.com", password: "newpassword" };
        const updatedUser: User = { id, email: data.email, password: data.password };

        (db.user.update as jest.Mock).mockReturnValueOnce(updatedUser);

        const result = await updateUser(id, data);

        expect(db.user.update).toHaveBeenCalledWith({
            where: { id },
            data,
        });
        expect(result).toEqual(updatedUser);
    });
});

describe("createUser", () => {
    it("should create a new user with the provided data", async () => {
        const data = { email: "newuser@example.com", password: "password" };
        const newUser: User = { id: "1", email: data.email, password: data.password };

        (db.user.create as jest.Mock).mockReturnValueOnce(newUser);

        const result = await createUser(data);

        expect(db.user.create).toHaveBeenCalledWith({
            data,
        });
        expect(result).toEqual(newUser);
    });
});

describe("getUserById", () => {
    it("should retrieve a user by its ID from the database", async () => {
        const user: User = { id: "1", email: "test@example.com", password: "password" };

        (db.user.findUnique as jest.Mock).mockReturnValueOnce(user);

        const result = await getUserById("1");

        expect(db.user.findUnique).toHaveBeenCalledWith({
            where: { id: "1" }
        });
        expect(result).toEqual(user);
    });

    it("should return null if the user is not found", async () => {
        (db.user.findUnique as jest.Mock).mockReturnValueOnce(null);

        const result = await getUserById("2");

        expect(db.user.findUnique).toHaveBeenCalledWith({
            where: { id: "2" }
        });
        expect(result).toBeNull();
    });
});

describe("getUserByEmail", () => {
    it("should retrieve a user by its email from the database", async () => {
        const user: User = { id: "1", email: "test@example.com", password: "password" };

        (db.user.findUnique as jest.Mock).mockReturnValueOnce(user);

        const result = await getUserByEmail("test@example.com");

        expect(db.user.findUnique).toHaveBeenCalledWith({
            where: { email: "test@example.com" }
        });
        expect(result).toEqual(user);
    });

    it("should return null if the user is not found", async () => {
        (db.user.findUnique as jest.Mock).mockReturnValueOnce(null);

        const result = await getUserByEmail("notfound@example.com");

        expect(db.user.findUnique).toHaveBeenCalledWith({
            where: { email: "notfound@example.com" }
        });
        expect(result).toBeNull();
    });
});

