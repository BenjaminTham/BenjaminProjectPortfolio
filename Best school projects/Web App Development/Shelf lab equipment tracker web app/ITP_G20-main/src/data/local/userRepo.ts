import {db} from "@/lib/db";
import {User} from "@prisma/client";

/**
 * Retrieves a user by its ID.
 * @param id - The ID of the user to retrieve.
 * @returns A promise that resolves to the user object if found, or null if not found.
 */
export async function getUserById(id: string): Promise<User | null> {
    return db.user.findUnique({
        where: {id},
    });
}

/**
 * Retrieves a user by its email.
 * @param email - The email of the user to retrieve.
 * @returns A Promise that resolves to the user object if found, or null if not found.
 */
export async function getUserByEmail(email: string): Promise<User | null> {
    return db.user.findUnique({
        where: {email},
    });
}

/**
 * Creates a new user with the provided data.
 * @param data - The data for the user, including the email and password.
 * @returns A Promise that resolves to the created user.
 */
export async function createUser(data: { email: string, password: string }): Promise<User> {
    return db.user.create({
        data,
    });
}

/**
 * Updates a user with the provided data.
 * @param id - The ID of the user to update.
 * @param data - The data to update the user with.
 * @returns A Promise that resolves to the updated user.
 */
export async function updateUser(id: string, data: { email: string, password: string }): Promise<User> {
    return db.user.update({
        where: {id},
        data,
    });
}

/**
 * Retrieves the number of users from the database.
 * @returns A Promise that resolves to the number of users.
 */
export async function getNumberOfUsers(): Promise<number> {
    return db.user.count();
}