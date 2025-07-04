import NextAuth from "next-auth";
import authConfig from "@/auth.config";
import {PrismaAdapter} from "@auth/prisma-adapter";
import {db} from "@/lib/db"
import {getUserById} from "@/data/local/userRepo"

export const {
    auth,
    handlers: {GET, POST},
    signIn,
    signOut
} = NextAuth({
    callbacks: {
        async session({token, session}) {
            if (token.sub && session.user) {
                session.user.id = token.sub;
            }

            if (session.user) {
                session.user.name = token.name!;
                session.user.email = token.email!;
            }

            return session;
        },
        async jwt({token}) {
            if (!token.sub) return token;

            const existingUser = await getUserById(token.sub);

            if (!existingUser) return token;

            token.email = existingUser.email;

            return token;
        },
    },
    adapter: PrismaAdapter(db),
    ...authConfig,
});