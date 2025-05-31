"use client"

import Link from "next/link";
import {Button} from "@/components/ui/button";
import {logout} from "@/domain/authControl";
import {User} from "next-auth";

interface AuthButtonProps {
    user?: User
}

export function LoginLogoutButton({
                                      user,
                                  }: AuthButtonProps) {
    async function handleLogout() {
        await logout();
    }

    if (user) {
        return (
            <Button onClick={handleLogout}>
                Logout
            </Button>
        );
    } else {
        return (
            <Button asChild>
                <Link href="/auth/login"> Login </Link>
            </Button>
        );
    }
}
