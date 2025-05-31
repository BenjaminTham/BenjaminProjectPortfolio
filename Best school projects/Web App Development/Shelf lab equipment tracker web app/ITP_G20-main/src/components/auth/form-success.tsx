import {CheckCircledIcon} from "@radix-ui/react-icons";
import React from "react";

interface FormSuccessProps {
    message?: string;
}

export const FormSuccess = ({
                                message,
                            }: FormSuccessProps) => {
    if (!message) return null;

    return (
        <div
            className="bg-primary text-primary-foreground hover:bg-primary/90 p-3 rounded-md flex items-center gap-x-2 text-sm">
            <CheckCircledIcon className="h-4 w-4"/>
            <p>{message}</p>
        </div>
    )
}