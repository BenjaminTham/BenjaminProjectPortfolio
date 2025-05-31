import {Function} from "@prisma/client";
import {z} from "zod";

export const systemStatusSchema = z.object({status: z.boolean()});

export const updateProfileSchema = z.object({
    email: z.string().email(),
    originalPassword: z.string().min(8),
    password: z.string().min(8),
    confirmPassword: z.string().min(8),
}).refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
});

export const createReaderFormSchema = z.object({
    ip: z.string().regex(/^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$/, {
        message: "Invalid IP address",
    }),
})

export const editHostIpSchema = z.object({
    ip: z.string().regex(/^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$/, {
        message: "Invalid IP address",
    }),
});

export const editAntennaFunctionSchena = z.object({
    antenna1: z.nativeEnum(Function),
    antenna2: z.nativeEnum(Function),
    antenna3: z.nativeEnum(Function),
    antenna4: z.nativeEnum(Function),
    antenna5: z.nativeEnum(Function),
    antenna6: z.nativeEnum(Function),
    antenna7: z.nativeEnum(Function),
    antenna8: z.nativeEnum(Function),
})


export const loginSchema = z.object({
    email: z.string().email({
        message: "Email is required"
    }),
    password: z.string().min(1, {
        message: "Password is required"
    }),
});

export const newUserSchema = z.object({
    email: z.string().email({
        message: "Email is required"
    }),
    password: z.string().min(8, {
        message: "Minimum 8 characters"
    }),
    confirmPassword: z.string().min(8, {
        message: "Minimum 8 characters"
    })
});

export const newRackSchema = z.object({
    location: z.string().min(1, {
        message: "Location is required"
    }),
    name: z.string().min(1, {
        message: "Name is required"
    })
});

export const updateRackSchema = z.object({
    rackId: z.number().min(1, {
        message: "Rack ID is required"
    }),
    location: z.string().min(1, {
        message: "Location is required"
    }),
    name : z.string().min(1, {
        message: "Name is required"
    })
});

export const editTraySchema = z.object({
    id: z.number().min(1, "Tray ID is required"),
    rackId: z.number().optional(),
});

export const createTraySchema = z.object({
    epc: z.string().min(1, "Tray EPC is required"),
    rackId: z.number().optional(),
});

export const powerLevelSchema = z.object({
    inventoryPower: z.coerce.number().min(1, "Inventory power must be greater than 1").max(30, "Inventory power must be less than 30"),
    geofencingPower: z.coerce.number().min(1, "Geofencing power must be greater than 1").max(30, "Geofencing power must be less than 30"),
    readwritePower: z.coerce.number().min(1, "Readwrite power must be greater than 1").max(30, "Readwrite power must be less than 30"),
});