import {PrismaClient} from "@prisma/client";

// Declare a global variable to hold the PrismaClient instance for preventing multiple instances of PrismaClient in development

declare global {
    var prisma: PrismaClient;
}

export const db = globalThis.prisma || new PrismaClient();

if (process.env.NODE_ENV === "development") {
    globalThis.prisma = db;
}