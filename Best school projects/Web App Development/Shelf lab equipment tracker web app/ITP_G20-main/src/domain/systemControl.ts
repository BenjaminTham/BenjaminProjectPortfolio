"use server";

import {addSystemStatus, getLatestSystemStatus} from "@/data/local/systemStatusRepo";
import {editHostIpSchema, powerLevelSchema} from "@/schema/custom";
import {z} from "zod";
import isPortReachable from "is-port-reachable";
import Conf from "conf";

const config = new Conf({projectName: 'SIT-RFID-System'});

/**
 * Retrieves the system status by calling the getLatestSystemStatus function.
 * @returns A promise that resolves to the latest system status.
 */
export async function retrieveSystemStatus() {
    const latestStatus = await getLatestSystemStatus();
    if (!latestStatus) {
        return logStatusChange(false);
    }
    return latestStatus;
}

/**
 * Logs the status change of the system.
 * @param status - The new status of the system.
 * @returns A promise that resolves with the result of adding the system status.
 */
export async function logStatusChange(status: boolean) {
    const systemStatusData = {
        systemStatus: status,
        systemTime: new Date()
    };
    console.log(systemStatusData);
    return await addSystemStatus(systemStatusData);
}

export async function getHostIpConfig(){
    return config.get('hostIp') as string | undefined;
}

export async function setHostIpConfig(value: z.infer<typeof editHostIpSchema>){
    const validatedFields = editHostIpSchema.safeParse(value);
    if (!validatedFields.success) {
        return {error: "Invalid fields!"};
    }

    const {ip} = validatedFields.data;

    const ipPortReachable = await isPortReachable(1883, {host: ip});

    if (!ipPortReachable) {
        return {error: "IP Address is not reachable!"};
    }
    
    config.set('hostIp', ip);
}

export async function setPowerLevelConfig(value: z.infer<typeof powerLevelSchema>){
    const validatedFields = powerLevelSchema.safeParse(value);
    if (!validatedFields.success) {
        return {error: "Invalid fields!"};
    }

    const powerLevel = validatedFields.data;

    config.set('powerLevel', powerLevel);
}

export async function getPowerLevelConfig(){
    return config.get('powerLevel') as z.infer<typeof powerLevelSchema> | undefined;
}