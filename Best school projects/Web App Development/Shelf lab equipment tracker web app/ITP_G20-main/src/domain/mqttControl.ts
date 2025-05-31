import mqtt from 'mqtt';
import {TagReportingDataAndIndexResponse} from '@/types/ura8';
import {updateTrayStatuses} from '@/domain/trayControl';
import {monitorGeofenceAntennas} from "@/domain/geofenceControl";
import {retrieveSystemStatus} from "@/domain/systemControl";

// This is to prevent the trays from being marked as OUT when they are actually IN
// If a tray is not detected for more than timeOut value, it will be marked as OUT
let tagHistory = new Map<string, Date>();
export const timeOut = 3000;

export async function startMQTTListener() {
    const client = mqtt.connect('mqtt://mqtt5:1883', {
        clientId: 'server',
        username: 'admin',
        password: process.env.MQTT_PASSWORD,
    });

    client.on('connect', () => {
        console.log('Connected to MQTT');
        global.lastMessageTime = new Date();
        // Subscribe to the topic 'reader'
        client.subscribe('reader', (err) => {
            if (err) {
                console.error('Subscription error:', err);
            } else {
                console.log('Subscribed to reader');
            }
        });
    });

    client.on('message', async (topic, message) => {
        global.lastMessageTime = new Date();
        const systemStatus = await retrieveSystemStatus();
        if (!systemStatus.systemStatus) {
            return;
        }
        const tagData = JSON.parse(message.toString()) as TagReportingDataAndIndexResponse;
        if (tagData.type !== "Reader-tagReportingEvent") {
            return;
        }
        const data = tagData.data;

        const epcs = data.map(tag => tag.epcHex);

        const currentTime = new Date();

        // Update the tag history
        epcs.forEach(epc => {
            tagHistory.set(epc, currentTime);
        });
        await updateTrayStatuses(epcs, tagHistory, currentTime, timeOut);
        //error: figure out what ip the antenna is currently using and pass it here
        await monitorGeofenceAntennas(data);
    });
}

