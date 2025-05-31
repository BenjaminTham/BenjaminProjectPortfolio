"use server";

import {createUser, getNumberOfUsers, getUserByEmail, updateUser} from "@/data/local/userRepo";
import {newUserSchema, updateProfileSchema} from "@/schema/custom";
import {z} from "zod";
import bcrypt from "bcryptjs";

/**
 * Updates the user profile with the provided values.
 *
 * @param values - The values to update the profile with.
 * @returns An object with an error property if there was an error during the update, otherwise undefined.
 */
export async function updateProfile(values: z.infer<typeof updateProfileSchema>) {
    const validation = updateProfileSchema.safeParse(values)

    if (!validation.success) {
        return {error: "Invalid input"}
    }

    const {email, originalPassword, password} = validation.data

    const existingUser = await getUserByEmail(email)

    if (!existingUser) {
        return {error: "User not found"}
    }

    const passwordMatch = await bcrypt.compare(originalPassword, existingUser.password)

    if (!passwordMatch) {
        return {error: "Incorrect password"}
    }

    const hashedPassword = await bcrypt.hash(password, 10)

    await updateUser(existingUser.id, {email, password: hashedPassword})
}

export async function checkNumberOfUsers() {
    return await getNumberOfUsers()
}

export const createNewUser = async (values: z.infer<typeof newUserSchema>) => {
    const validatedFields = newUserSchema.safeParse(values);
    if (!validatedFields.success) {
        return {error: "Invalid fields!"};
    }

    const {email, password, confirmPassword} = validatedFields.data;

    if (password !== confirmPassword) {
        return {error: "Passwords do not match!"};
    }

    const hashedPassword = await bcrypt.hash(password, 10);

    const existingUser = await getUserByEmail(email);

    if (existingUser) {
        return {error: "Email already in use!"};
    }

    await createUser({
        email: email,
        password: hashedPassword,
    });

    // Add any other business logic here (e.g., checking credentials)
    return {success: "User created."};
};
