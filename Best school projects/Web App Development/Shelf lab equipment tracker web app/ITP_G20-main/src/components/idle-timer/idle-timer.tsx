"use client";
import { useIdleTimer } from 'react-idle-timer';
import { logout } from "@/domain/authControl";
import { useState } from "react";
import {
    AlertDialog,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle
} from "@/components/ui/alert-dialog";
import { User } from "next-auth";

interface IdleTimerProps {
    user?: User;
}

export default function IdleTimer({ user }: IdleTimerProps) {
    const [isIdle, setIsIdle] = useState(false);

    const onIdle = async () => {
        setIsIdle(true);
        await logout();
    }

    const handleClose = () => {
        setIsIdle(false);
    }

    useIdleTimer({
        onIdle,
        timeout: 3 * 60 * 1000, // 3 minutes
        throttle: 1000,
    });

    if (!user) {
        return null;
    }

    return (
        <AlertDialog open={isIdle} onOpenChange={handleClose}>
            <AlertDialogContent>
                <AlertDialogHeader>
                    <AlertDialogTitle>Your session has expired</AlertDialogTitle>
                    <AlertDialogDescription>
                        You have been idle for too long. Please log in again.
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel onClick={handleClose}>Continue</AlertDialogCancel>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    );
}