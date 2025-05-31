import bcrypt from "bcryptjs"
import type {NextAuthConfig} from "next-auth"
import {loginSchema} from "@/schema/custom"
import {getUserByEmail} from "@/data/local/userRepo"
import Credentials from "next-auth/providers/credentials"

export default {
    session: {
        strategy: "jwt",
        maxAge: 3 * 60
    },
    providers: [
        Credentials({
            async authorize(credentials) {
                const validatedFields = loginSchema.safeParse(credentials)
                if (validatedFields.success) {
                    const {email, password} = validatedFields.data

                    const user = await getUserByEmail(email)

                    if (!user) return null

                    const isPasswordMatch = await bcrypt.compare(password, user.password)

                    if (isPasswordMatch) return user
                }

                return null
            }
        }),
    ]
} satisfies NextAuthConfig