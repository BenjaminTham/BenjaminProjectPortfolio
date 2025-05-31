import React from "react";
import test from "node:test";
import { beforeEach, describe, expect, it, jest } from "@jest/globals";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";

import { checkNumberOfUsers, createNewUser, updateProfile } from "../domain/userControl";
import { createUser, getNumberOfUsers, getUserByEmail, updateUser } from "../data/local/userRepo";
import { newUserSchema } from "../schema/custom";
import bcrypt from 'bcryptjs';

/**
 * Jest describe and it(test) indicators
 * [<describe/test(it)>] [<function/file>] [<function name/file name>] test description (OPTIONAL)
 */
jest.mock("@/data/local/userRepo", () => ({
    getNumberOfUsers: jest.fn(),
    getUserByEmail: jest.fn(),
    createUser: jest.fn(),
    updateUser: jest.fn(),
}));

jest.mock('bcryptjs', () => ({
    hash: jest.fn(),
    compare: jest.fn(),
}));

describe('[DESCRIBE][FILE][userControl]', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('[DESCRIBE][FUNC][createNewUser]', () => {
        beforeEach(() => {
            jest.clearAllMocks();
        });

        it('[TEST][FUNC][createNewUser] error when fields are invalid', async () => {
            const invalidValues = {
                email: 'email email email',
                password: '123',
                confirmPassword: '123',
            };

            const result = await createNewUser(invalidValues);

            expect(result).toEqual({ error: 'Invalid fields!' });
        });

        it('[TEST][FUNC][createNewUser] error when passwords do not match', async () => {
            const values = {
                email: 'test@example.com',
                password: 'password123',
                confirmPassword: 'password124',
            };

            const result = await createNewUser(values);

            expect(result).toEqual({ error: 'Passwords do not match!' });
        });

        it('[TEST][FUNC][createNewUser] error when email is already in use', async () => {
            const values = {
                email: 'test@example.com',
                password: 'password123',
                confirmPassword: 'password123',
            };

            (getUserByEmail as jest.Mock).mockReturnValueOnce({ email: 'test@example.com' });

            const result = await createNewUser(values);

            expect(getUserByEmail).toHaveBeenCalledWith(values.email);
            expect(result).toEqual({ error: 'Email already in use!' });
        });

        it('[TEST][FUNC][createNewUser] create a new user', async () => {
            const values = {
                email: 'createnewuser@example.com',
                password: 'password123',
                confirmPassword: 'password123',
            };

            (getUserByEmail as jest.MockedFunction<typeof getUserByEmail>).mockResolvedValue(null);
            const spiedBcryptHashMethod = jest.spyOn(bcrypt, 'hash').mockImplementation((pass, salt, cb) => {
                if (cb) cb(null, 'hashedPassword');
                return Promise.resolve('hashedPassword');
            });
            (createUser as jest.MockedFunction<typeof createUser>).mockResolvedValue({ id: '1', email: 'createnewuser@example.com', password: 'hashedPassword' });

            const result = await createNewUser(values);

            expect(getUserByEmail).toHaveBeenCalledWith(values.email);
            expect(spiedBcryptHashMethod).toHaveBeenCalledWith(values.password, 10);
            expect(createUser).toHaveBeenCalledWith({
                email: values.email,
                password: 'hashedPassword',
            });
            expect(result).toEqual({ success: 'User created.' });
        });
    });

    describe('[DESCRIBE][FUNC][checkNumberOfUsers]', () => {
        beforeEach(() => {
            jest.clearAllMocks();
        });

        it("[TEST][FUNC][checkNumberOfUsers] should call getNumberOfUsers", async () => {
            await checkNumberOfUsers();
            expect(getNumberOfUsers).toHaveBeenCalled();
        });
    })

    describe('[DESCRIBE][FUNC][updateProfile]', () => {
        it('[TEST][FUNC][updateProfile] returns error when validation fails', async () => {
            const invalidValues = {
                email: 'invalid-email',
                originalPassword: 'password123',
                password: 'newpassword123',
                confirmPassword: 'newpassword123',
            };

            const result = await updateProfile(invalidValues);

            expect(result).toEqual({ error: 'Invalid input' });
        });

        it('[TEST][FUNC][updateProfile]returns error when user is not found', async () => {
            const values = {
                email: 'test@example.com',
                originalPassword: 'password123',
                password: 'newpassword123',
                confirmPassword: 'newpassword123',
            };

            (getUserByEmail as jest.MockedFunction<typeof getUserByEmail>).mockResolvedValue(null);

            const result = await updateProfile(values);

            expect(getUserByEmail).toHaveBeenCalledWith(values.email);
            expect(result).toEqual({ error: 'User not found' });
        });

        it('[TEST][FUNC][updateProfile] returns error when original password does not match', async () => {
            const values = {
                email: 'test@example.com',
                originalPassword: 'password123',
                password: 'newpassword123',
                confirmPassword: 'newpassword123',
            };

            // (getUserByEmail as jest.MockedFunction<typeof getUserByEmail>).mockResolvedValue({ email: 'test@example.com', password: 'hashedpassword' });
            (getUserByEmail as jest.Mock).mockReturnValueOnce({ email: 'test@example.com', password: 'hashedpassword' });

            const spiedBcryptCompareMethod = jest.spyOn(bcrypt, 'compare').mockImplementation(() => Promise.resolve(false))

            const result = await updateProfile(values);

            expect(getUserByEmail).toHaveBeenCalledWith(values.email);
            expect(spiedBcryptCompareMethod).toHaveBeenCalledWith(values.originalPassword, 'hashedpassword');
            expect(result).toEqual({ error: 'Incorrect password' });
        });

        it('[TEST][FUNC][updateProfile] updates the user profile successfully', async () => {
            const values = {
                email: 'test@example.com',
                originalPassword: 'password123',
                password: 'newpassword123',
                confirmPassword: 'newpassword123',
            };

            (getUserByEmail as jest.MockedFunction<typeof getUserByEmail>).mockResolvedValue({ id: '1', email: 'test@example.com', password: 'hashedpassword' });
            const spiedBcryptCompareMethod = jest.spyOn(bcrypt, 'compare').mockImplementation(() => Promise.resolve(true));
            const spiedBcryptHashMethod = jest.spyOn(bcrypt, 'hash').mockImplementation((pass, salt, cb) => {
                if (cb) cb(null, 'newhashedpassword');
                return Promise.resolve('newhashedpassword');
            });
            (updateUser as jest.MockedFunction<typeof updateUser>).mockResolvedValue({ id: '1', email: 'test@example.com', password: 'newhashedpassword' });

            const result = await updateProfile(values);

            expect(getUserByEmail).toHaveBeenCalledWith(values.email);
            expect(spiedBcryptCompareMethod).toHaveBeenCalledWith(values.originalPassword, 'hashedpassword');
            expect(spiedBcryptHashMethod).toHaveBeenCalledWith(values.password, 10);
            expect(updateUser).toHaveBeenCalledWith('1', {
                email: values.email,
                password: 'newhashedpassword',
            });
            expect(result).toEqual(undefined);
        });
    });
});