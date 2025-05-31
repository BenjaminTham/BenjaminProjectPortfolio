"use server"

import * as z from "zod";
import {loginSchema} from "@/schema/custom";
import {getUserByEmail} from "@/data/local/userRepo";
import {signIn, signOut} from "@/auth";
import {DEFAULT_LOGIN_REDIRECT} from "@/routes";
import {AuthError} from "next-auth";

export const authenticateUser = async (values: z.infer<typeof loginSchema>) => {
    const validatedFields = loginSchema.safeParse(values);
    if (!validatedFields.success) {
        return {error: "Invalid fields!"};
    }

    const {email, password} = validatedFields.data;

    const existingUser = await getUserByEmail(email);

    if (!existingUser || !existingUser.email) {
        return {error: "Invalid credentials!"}
    }

    try {
        await signIn("credentials", {
            email, password, redirectTo: DEFAULT_LOGIN_REDIRECT,
        })
        return {success: "Login Successful."}

    } catch (error) {
        if (error instanceof AuthError) {
            console.log("[authcontrol-ts][error-type]: ", error.type)
            switch (error.type) {
                case "CredentialsSignin":
                    return {error: "Invalid credentials!"}
                default:
                    return {error: "Something went wrong!"}
            }
        }
        throw error;
    }
};

export const logout = async () => {
    await signOut({redirectTo: "/", redirect: true})
}