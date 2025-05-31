//src/domain/geofenceControl.ts

import {Function} from '@prisma/client';
import {TagReportingData} from '@/types/ura8';
import {getReadersWithGeofenceAntennas as getReadersWithGeofenceAntenna} from '@/data/local/readerRepo';
import {setTrayOutOfBound} from "@/domain/trayControl";

/**
 * Monitors geofence antennas for new data and logs when trays move out of bounds.
 */
export async function monitorGeofenceAntennas(data: TagReportingData[]) {
    const reader = await getReadersWithGeofenceAntenna()

    if (!reader) {
        console.log('No readers with geofence antennas found.');
        return;
    }

    const geofenceAntennas = reader.antennas.filter(antenna => antenna.function === Function.GEOFENCE);

    // Check if the tag has been detected by a geofence antenna
    const geofenceTags = data.filter(tag => geofenceAntennas.some(antenna => antenna.antennaPort === tag.antennaPort));
    const epcsDetectedByGeofence = geofenceTags.map(tag => tag.epcHex);

    if (epcsDetectedByGeofence.length === 0) {
        return;
    }
    console.log('Trays detected by geofence antennas:', epcsDetectedByGeofence);
    epcsDetectedByGeofence.forEach(epc => {
        setTrayOutOfBound(epc, true);
    });
}