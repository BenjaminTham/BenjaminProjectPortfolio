"use client";

import React from "react";
import {Card, CardContent, CardHeader, CardTitle} from "@/components/ui/card";

interface CardWrapperProps {
    children: React.ReactNode;
    headerLabel: string;
    backButtonLabel: string;
    backButtonHref: string;
    showSocial?: boolean;
}

export const CardWrapper = ({
                                children,
                                headerLabel,
                                backButtonLabel,
                                backButtonHref,
                            }: CardWrapperProps) => {
    return (
        <Card className="w-[400px] shadow-md">
            <CardHeader>
                <CardTitle className="text-center">{headerLabel}</CardTitle>
            </CardHeader>
            <CardContent>
                {children}
            </CardContent>
        </Card>
    )
}