import {auth} from "@/auth";
import {User} from "next-auth";

/**
 * Retrieves the current user from the authentication session.
 *
 * @return {Promise<User | undefined>} The current user if available, otherwise undefined.
 */
export const currentUser = async (): Promise<User | undefined> => {
    const session = await auth();

    return session?.user;
};