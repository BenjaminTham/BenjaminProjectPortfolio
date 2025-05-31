//src/data/remote/ura8.ts

import {
    QueryAfterNetworkDisconnectedResponse,
    QueryAntennaEnabledStateResponse,
    QueryAntennaPowerResponse,
    QueryBuzzerForInventoryResponse,
    QueryFastIDResponse,
    QueryFrequencyResponse,
    QueryHeartbeatPacketResponse,
    QueryInventoryMemoryBankResponse,
    QueryInventorySearchModeResponse,
    QueryLinkFrequencyResponse,
    queryMqttConfigurationResponse,
    QueryTagfocusResponse,
    QueryTemperatureResponse,
    StartInventoryResponse,
    StopInventoryResponse,
    TagReadReponse,
    TagReportingDataAndIndexResponse,
    TagWriteResponse,
    UpdateAfterNetworkDisconnectedResponse,
    UpdateAntennaEnabledStateResponse,
    UpdateAntennaPowerResponse,
    UpdateBuzzerForInventoryResponse,
    UpdateFastIDResponse,
    UpdateFrequencyResponse,
    UpdateHeartbeatPacketResponse,
    UpdateInventoryMemoryBankResponse,
    UpdateInventorySearchModeResponse,
    UpdateLinkFrequencyResponse,
    UpdateMqttConfigurationResponse,
    UpdateTagfocusResponse,
    UpdateTagReportingResponse
} from "@/types/ura8"

/**
 * Sends a POST request to the specified URL with the provided data.
 * @param url - The URL to send the request to.
 * @param data - The data to be sent in the request body.
 * @returns A Promise that resolves to the response data.
 * @throws If an HTTP error occurs or if there is an error parsing the response data.
 */
async function sendPostRequest(url: string, data: any): Promise<any> {
    const urlEncodedData = `data=${encodeURIComponent(JSON.stringify(data))}`;

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            },
            body: urlEncodedData
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const responseData = await response.json();
        return responseData;
    } catch (error) {
        console.error("Error:", error);
        throw error;
    }
}

/**
 * Queries the antenna power for a given IP address.
 * @param ip The IP address of the device.
 * @returns A promise that resolves to a QueryAntennaPowerResponse object.
 */
export async function queryAntennaPower(ip: string): Promise<QueryAntennaPowerResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "antennaPower"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryAntennaPowerResponse;
}

/**
 * Queries the enabled state of the antenna for a given IP address.
 * @param ip The IP address of the device.
 * @returns A promise that resolves to a QueryAntennaEnabledStateResponse object.
 */
export async function queryAntennaEnabledState(ip: string): Promise<QueryAntennaEnabledStateResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "antennaEnabledState"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryAntennaEnabledStateResponse;
}

/**
 * Queries the frequency for a given IP address.
 * @param ip The IP address to query.
 * @returns A promise that resolves to a QueryFrequencyResponse object.
 */
export async function queryFrequency(ip: string): Promise<QueryFrequencyResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "frequency"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryFrequencyResponse;
}

/**
 * Queries the inventory search mode for a given IP address.
 * @param ip The IP address of the device.
 * @returns A promise that resolves to a QueryInventorySearchModeResponse object.
 */
export async function queryInventorySearchMode(ip: string): Promise<QueryInventorySearchModeResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "inventorySearchMode"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryInventorySearchModeResponse;
}

/**
 * Queries the inventory memory bank for a given IP address.
 * @param ip The IP address of the device.
 * @returns A promise that resolves to a QueryInventoryMemoryBankResponse object.
 */
export async function queryInventoryMemoryBank(ip: string): Promise<QueryInventoryMemoryBankResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "inventoryMemoryBank"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryInventoryMemoryBankResponse;
}

/**
 * Queries the tag reporting configuration from a specific IP address.
 * @param ip The IP address of the device.
 * @returns A promise that resolves to a QueryInventoryMemoryBankResponse object.
 */
export async function queryTagReporting(ip: string): Promise<QueryInventoryMemoryBankResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "tagReporting"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryInventoryMemoryBankResponse;
}

/**
 * Queries the heartbeat packet for a given IP address.
 * @param ip - The IP address to query.
 * @returns A promise that resolves to the response containing the heartbeat packet.
 */
export async function queryHeartbeatPacket(ip: string): Promise<QueryHeartbeatPacketResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "heartbeatPacket"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryHeartbeatPacketResponse;
}

/**
 * Queries the buzzer for inventory based on the provided IP address.
 * @param ip The IP address of the buzzer.
 * @returns A promise that resolves to a QueryBuzzerForInventoryResponse object.
 */
export async function queryBuzzerForInventory(ip: string): Promise<QueryBuzzerForInventoryResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "buzzerForInventory"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryBuzzerForInventoryResponse;
}

/**
 * Queries the tag focus configuration from the specified IP address.
 * @param ip The IP address of the device.
 * @returns A promise that resolves to the response containing the tag focus configuration.
 */
export async function queryTagFocus(ip: string): Promise<QueryTagfocusResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "tagfocus"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryTagfocusResponse;
}

/**
 * Queries the fast ID from the specified IP address.
 * @param ip The IP address to query.
 * @returns A promise that resolves to the response containing the fast ID.
 */
export async function queryFastID(ip: string): Promise<QueryFastIDResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "fastID"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryFastIDResponse;
}

/**
 * Queries the link frequency for a given IP address.
 * @param ip The IP address to query.
 * @returns A promise that resolves to a QueryLinkFrequencyResponse object.
 */
export async function queryLinkFrequency(ip: string): Promise<QueryLinkFrequencyResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "linkFrequency"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryLinkFrequencyResponse;
}

/**
 * Queries the base configuration after the network is disconnected.
 * @param ip - The IP address of the device.
 * @returns A promise that resolves to a QueryAfterNetworkDisconnectedResponse object.
 */
export async function queryAfterNetworkDisconnected(ip: string): Promise<QueryAfterNetworkDisconnectedResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/queryBaseConfigurationRequest`;

    const data = {
        type: "Reader-queryBaseConfigurationRequest",
        functionList: [
            {name: "afterNetworkDisconnected"}
        ]
    };

    const response = await sendPostRequest(url, data);
    return response as QueryAfterNetworkDisconnectedResponse;
}

/**
 * Updates the antenna power for a given IP address.
 * @param ip The IP address of the device.
 * @param antennaPower An array of objects representing the antenna port and power level.
 * @returns A promise that resolves to the response of the update operation.
 */
export async function updateAntennaPower(
    ip: string,
    antennaPower = [
        {antennaPort: 1, power: 30},
        {antennaPort: 2, power: 30},
        {antennaPort: 3, power: 30},
        {antennaPort: 4, power: 30},
        {antennaPort: 5, power: 30},
        {antennaPort: 6, power: 30},
        {antennaPort: 7, power: 30},
        {antennaPort: 8, power: 30}
    ]
): Promise<UpdateAntennaPowerResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    console.log(antennaPower);

    for (const {power} of antennaPower) {
        if (power < 1 || power > 30) {
            throw new Error("Antenna power must be between 1 and 30");
        }
    }

    for (const {antennaPort} of antennaPower) {
        if (antennaPort < 1 || antennaPort > 8) {
            throw new Error("Antenna port must be between 1 and 8");
        }
    }

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        antennaPower
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateAntennaPowerResponse;
}

/**
 * Updates the enabled state of the antennas for a given IP address.
 * @param ip The IP address of the device.
 * @param antennaEnabledState An array of antenna enabled states.
 * @returns A promise that resolves to the response of the update operation.
 * @throws An error if the antenna port is not between 1 and 8.
 */
export async function updateAntennaEnabledState(
    ip: string,
    antennaEnabledState = [
        {antennaPort: 1, enable: false},
        {antennaPort: 2, enable: false},
        {antennaPort: 3, enable: false},
        {antennaPort: 4, enable: false},
        {antennaPort: 5, enable: false},
        {antennaPort: 6, enable: false},
        {antennaPort: 7, enable: false},
        {antennaPort: 8, enable: false}
    ]
): Promise<UpdateAntennaEnabledStateResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    for (const {antennaPort} of antennaEnabledState) {
        if (antennaPort < 1 || antennaPort > 8) {
            throw new Error("Antenna port must be between 1 and 8");
        }
    }

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        antennaEnabledState
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateAntennaEnabledStateResponse;
}

/**
 * Updates the frequency of a device.
 * @param ip The IP address of the device.
 * @param frequency The frequency to be set. Must be one of the following values:
 * - Option 1: China Standard(840~845MHz)
 * - Option 2: China Standard2(920~925MHz)
 * - Option 3: Europe Standard(865~868MHz)
 * - Option 8: USA(902-928MHz)
 * - Option 22: Korea(917~923MHz)
 * - Option 50: Japan(952~953MHz)
 * @returns A promise that resolves to an UpdateFrequencyResponse object.
 */
export async function updateFrequency(
    ip: string,
    frequency: 1 | 2 | 3 | 8 | 22 | 50
): Promise<UpdateFrequencyResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        "frequency": {"region": frequency}
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateFrequencyResponse;
}

/**
 * Updates the inventory search mode for a given IP address.
 *
 * @param ip - The IP address of the device.
 * @param queryTarget - The query target ("A" or "B").
 * @param querySession - The query session ("S1", "S2", "S3", or "S4").
 * @returns A promise that resolves to the response of the update operation.
 */
export async function updateInventorySearchMode(
    ip: string,
    queryTarget: "A" | "B",
    querySession: "S1" | "S2" | "S3" | "S4"
): Promise<UpdateInventorySearchModeResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        inventorySearchMode: {queryTarget, querySession}
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateInventorySearchModeResponse;
}

/**
 * Updates the inventory memory bank configuration for a given IP address.
 *
 * @param ip - The IP address of the device.
 * @param memoryBank - The memory bank to update. Can be "epc", "epc+tid", or "epc+tid+user".
 * @param wordUserOffset - The offset of the user memory bank. Defaults to 0. Only used if memoryBank is "epc+tid+user".
 * @param wordUserLength - The length of the user memory bank. Defaults to 0. Only used if memoryBank is "epc+tid+user".
 * @returns A promise that resolves to the response of the update operation.
 */
export async function updateInventoryMemoryBank(
    ip: string,
    memoryBank: "epc" | "epc+tid" | "epc+tid+user",
    wordUserOffset: number = 0,
    wordUserLength: number = 0
): Promise<UpdateInventoryMemoryBankResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    if (memoryBank === "epc" || memoryBank === "epc+tid") {
        wordUserOffset = 0;
        wordUserLength = 0;
    }

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        inventoryMemoryBank: {
            memoryBank,
            wordUserOffset: wordUserOffset.toString(),
            wordUserLength: wordUserLength.toString()
        }
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateInventoryMemoryBankResponse;
}

/**
 * Updates the tag reporting configuration on the specified IP address.
 * @param ip - The IP address of the device.
 * @param duplicateTagFilterEnable - A boolean indicating whether duplicate tag filtering is enabled.
 * @param filterByTime - The time (in milliseconds) to filter duplicate tags.
 * @param rssiFilterEnable - A boolean indicating whether RSSI filtering is enabled.
 * @param rssiIsGreateThan - The RSSI threshold value.
 * @param tagReportingMode - The tag reporting mode, either "realTime" or "afterStopInventory".
 * @returns A promise that resolves to the response of the update operation.
 */
export async function updateTagReporting(
    ip: string,
    duplicateTagFilterEnable: boolean,
    filterByTime: number,
    rssiFilterEnable: boolean,
    rssiIsGreateThan: number,
    tagReportingMode: "realTime" | "afterStopInventory"
): Promise<UpdateTagReportingResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    if (rssiIsGreateThan > 0) {
        throw new Error("rssiIsGreateThan must be less than 0");
    }

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        tagReporting: {
            duplicateTagFilter: {
                enable: duplicateTagFilterEnable,
                filterByTime
            },
            rssiFilter: {
                enable: rssiFilterEnable,
                rssiIsGreateThan: rssiIsGreateThan.toString()
            },
            tagReportingMode: {mode: tagReportingMode}
        }
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateTagReportingResponse;
}

/**
 * Updates the heartbeat packet configuration for a given IP address.
 * @param ip - The IP address of the device.
 * @param heartbeatPacketEnable - A boolean indicating whether the heartbeat packet is enabled.
 * @param intervalTime - The interval time for the heartbeat packet in milliseconds.
 * @returns A Promise that resolves to an UpdateHeartbeatPacketResponse object.
 */
export async function updateHeartbeatPacket(
    ip: string,
    heartbeatPacketEnable: boolean,
    intervalTime: number,
): Promise<UpdateHeartbeatPacketResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        heartbeatPacket: {enable: heartbeatPacketEnable, intervalTime}
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateHeartbeatPacketResponse;
}

/**
 * Updates the link frequency of a device.
 * @param ip - The IP address of the device.
 * @param linkFrequency - The desired link frequency. Must be one of the following values:
 *   - "DSB_ASK_FM0_40KHz"
 *   - "PR_ASK_Miller4_250KHz"
 *   - "PR_ASK_Miller4_300KHz"
 *   - "DSB_ASK_FM0_400KHz"
 * @returns A promise that resolves to an UpdateLinkFrequencyResponse object.
 */
export async function updateLinkFrequency(
    ip: string,
    linkFrequency: "DSB_ASK_FM0_40KHz" | "PR_ASK_Miller4_250KHz" | "PR_ASK_Miller4_300KHz" | "DSB_ASK_FM0_400KHz"
): Promise<UpdateLinkFrequencyResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        linkFrequency: {value: linkFrequency}
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateLinkFrequencyResponse;
}

/**
 * Updates the buzzer configuration for inventory on the specified IP address.
 * @param ip - The IP address of the device.
 * @param buzzerForInventory - A boolean value indicating whether the buzzer for inventory is enabled or disabled.
 * @returns A promise that resolves to an UpdateBuzzerForInventoryResponse object.
 */
export async function updateBuzzerForInventory(
    ip: string,
    buzzerForInventory: boolean,
): Promise<UpdateBuzzerForInventoryResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        buzzerForInventory: {enable: buzzerForInventory},
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateBuzzerForInventoryResponse;
}

/**
 * Updates the tag focus configuration on a specific IP address.
 * @param ip The IP address of the device.
 * @param tagFocus A boolean value indicating whether tag focus is enabled or disabled.
 * @returns A promise that resolves to an `UpdateTagfocusResponse` object.
 */
export async function updateTagFocus(
    ip: string,
    tagFocus: boolean,
): Promise<UpdateTagfocusResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        tagFocus: {enable: tagFocus},
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateTagfocusResponse;
}

/**
 * Updates the fastID configuration on a specific IP address.
 * @param ip The IP address of the device.
 * @param fastID A boolean value indicating whether fastID should be enabled or disabled.
 * @returns A promise that resolves to an `UpdateFastIDResponse` object.
 */
export async function updateFastID(
    ip: string,
    fastID: boolean,
): Promise<UpdateFastIDResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        fastID: {enable: fastID},
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateFastIDResponse;
}

/**
 * Updates the base configuration after network disconnection.
 *
 * @param ip - The IP address of the device.
 * @param cacheTags - A boolean indicating whether to cache tags.
 * @returns A promise that resolves to the response after updating the base configuration.
 */
export async function updateAfterNetworkDisconnected(
    ip: string,
    cacheTags: boolean,
): Promise<UpdateAfterNetworkDisconnectedResponse> {
    const url = `http://${ip}:8080/BaseConfigurationController/updateBaseConfigurationRequest`;

    const data = {
        type: "Reader-updateBaseConfigurationRequest",
        cacheTags: {cacheTags}
    };

    const response = await sendPostRequest(url, data);
    return response as UpdateAfterNetworkDisconnectedResponse;
}

/**
 * Sends a start inventory request to the specified IP address.
 * The scan will stop it after a few seconds if {@link stopInventoryRequest} is not called.
 * Is it recommended to call {@link stopInventoryRequest} after calling this function.
 * @param ip - The IP address of the reader.
 * @param enableFilter - A boolean indicating whether to enable tag filtering.
 * @param tagMemoryBank - The memory bank to use for tag filtering ("epc", "tid", or "user").
 * @param bitOffset - The bit offset for tag filtering.
 * @param bitLength - The bit length for tag filtering.
 * @param hexMask - The hex mask for tag filtering.
 * @returns A promise that resolves to a StartInventoryResponse object.
 */
export async function startInventoryRequest(
    ip: string,
    enableFilter: boolean = false,
    tagMemoryBank: "epc" | "tid" | "user" = "epc",
    bitOffset: number = 2,
    bitLength: number = 96,
    hexMask: string = "00000000"
): Promise<StartInventoryResponse> {
    const url = `http://${ip}:8080/InventoryController/startInventoryRequest`

    const data = {
        type: "Reader-startInventoryRequest",
        tagFilter: {
            tagMemoryBank,
            bitOffset,
            "bitLength": enableFilter ? bitLength : 0,
            "hexMask": enableFilter ? hexMask : null
        }
    }

    const response = await sendPostRequest(url, data)
    return response as StartInventoryResponse
}

/**
 * Sends a POST request to the specified IP address to retrieve tag reporting data and index.
 * @param ip - The IP address to send the request to.
 * @returns A promise that resolves to the response data containing tag reporting data and index.
 * @throws If there is an HTTP error or an error occurs during the request.
 */
export async function tagReportingDataAndIndex(ip: string): Promise<TagReportingDataAndIndexResponse> {
    const url = `http://${ip}:8080/InventoryController/tagReportingDataAndIndex`

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            }
        })

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`)
        }

        const responseData = await response.json()
        return responseData as TagReportingDataAndIndexResponse
    } catch (error) {
        console.error("Error:", error);
        throw error;
    }
}

/**
 * Stops the inventory request for the specified IP address.
 * @param ip - The IP address of the device.
 * @returns A promise that resolves to a `StopInventoryResponse` object.
 */
export async function stopInventoryRequest(ip: string): Promise<StopInventoryResponse> {
    const url = `http://${ip}:8080/InventoryController/stopInventoryRequest`

    const data = {type: "Reader-stopInventoryRequest"}

    const response = await sendPostRequest(url, data)
    return response as StopInventoryResponse
}

/**
 * Sends a tag read request to the specified IP address.
 *
 * @param ip - The IP address of the device.
 * @param hexAccessPassword - The access password in hexadecimal format. Default is "00000000".
 * @param tagMemoryBankParameter - The tag memory bank parameter ("epc", "tid", "user", or "reserver").
 * @param wordOffset - The word offset.
 * @param wordLength - The word length.
 * @param enableFilter - Indicates whether to enable tag filtering.
 * @param tagMemoryBankFilter - The tag memory bank for filtering ("epc", "tid", or "user").
 * @param bitOffset - The bit offset for filtering.
 * @param bitLength - The bit length for filtering.
 * @param hexMask - The mask in hexadecimal format for filtering.
 * @returns A promise that resolves to the tag read response.
 */
export async function tagReadRequest(
    ip: string,
    hexAccessPassword: string = "00000000",
    tagMemoryBankParameter: "epc" | "tid" | "user" | "reserver" = "epc",
    wordOffset: number = 2,
    wordLength: number = 6,
    enableFilter: boolean = false,
    tagMemoryBankFilter: "epc" | "tid" | "user" = "epc",
    bitOffset: number = 32,
    bitLength: number = 96,
    hexMask: string = "00000000"
): Promise<TagReadReponse> {
    const url = `http://${ip}:8080/ReadController/tagReadRequest`

    const data = enableFilter ?
        {
            type: "Reader-tagReadRequest",
            tagParameters: {
                hexAccessPassword,
                tagMemoryBank: tagMemoryBankParameter,
                wordOffset,
                wordLength
            },
            tagFilter: {
                tagMemoryBank: tagMemoryBankFilter,
                bitOffset,
                bitLength,
                hexMask,
            }
        }
        :
        {
            type: "Reader-tagReadRequest",
            tagParameters: {
                hexAccessPassword,
                tagMemoryBank: tagMemoryBankParameter,
                wordOffset,
                wordLength
            }
        }


    const response = await sendPostRequest(url, data)
    return response as TagReadReponse
}

/**
 * Sends a tag write request to a specified IP address.
 * @param ip The IP address of the target device.
 * @param hexAccessPassword The access password in hexadecimal format. Default is "00000000".
 * @param tagMemoryBank The memory bank to write the tag data to. Can be "epc", "tid", "user", or "reserver".
 * @param wordOffset The starting word offset in the memory bank.
 * @param wordLength The number of words to write.
 * @param hexTagData The tag data to write in hexadecimal format.
 * @param enableFilter Specifies whether to enable tag filtering.
 * @param tagMemoryBankFilter The memory bank to use for tag filtering. Can be "epc", "tid", or "user".
 * @param bitOffset The starting bit offset in the memory bank for tag filtering.
 * @param bitLength The number of bits to compare for tag filtering.
 * @param hexMask The mask to apply for tag filtering in hexadecimal format.
 * @returns A promise that resolves to a TagWriteResponse object.
 */
export async function tagWriteRequest(
    ip: string,
    hexTagData: string,
    hexAccessPassword: string = "00000000",
    tagMemoryBankParameter: "epc" | "tid" | "user" | "reserver" = "epc",
    wordOffset: number = 2,
    wordLength: number = 6,
    enableFilter: boolean = false,
    tagMemoryBankFilter: "epc" | "tid" | "user" = "epc",
    bitOffset: number = 32,
    bitLength: number = 96,
    hexMask: string = "00000000"
): Promise<TagWriteResponse> {
    const url = `http://${ip}:8080/WriteController/tagWriteRequest`

    const data = enableFilter ?
        {
            type: "Reader-tagWriteRequest",
            tagParameters:
                {
                    hexAccessPassword,
                    tagMemoryBank: tagMemoryBankParameter,
                    wordOffset,
                    wordLength,
                    hexTagData
                },
            tagFilter: {
                tagMemoryBank: tagMemoryBankFilter,
                bitOffset,
                bitLength,
                hexMask,
            }
        }
        :
        {
            type: "Reader-tagWriteRequest",
            tagParameters:
                {
                    hexAccessPassword,
                    tagMemoryBank: tagMemoryBankParameter,
                    wordOffset,
                    wordLength,
                    hexTagData
                }
        }

    console.log (data)

    const response = await sendPostRequest(url, data)
    return response as TagWriteResponse
}

/**
 * Sends a query request to the MQTT configuration controller.
 * @param ip The IP address of the MQTT configuration controller.
 * @returns A promise that resolves to the response from the MQTT configuration controller.
 */
export async function queryMqttConfigurationRequest(ip: string): Promise<queryMqttConfigurationResponse> {
    const url = `http://${ip}:8080/MqttConfigurationController/queryMqttConfigurationRequest`

    const data = {
        type: "Reader-queryMqttConfigurationRequest",
        functionList: [
            {name: "normalConfiguration"}
        ]
    }

    const response = await sendPostRequest(url, data) as queryMqttConfigurationResponse
    return response
}

/**
 * Updates the MQTT configuration request.
 * @param ip - The IP address of the MQTT server.
 * @param enable - A boolean indicating whether MQTT is enabled or not.
 * @param hostName - The host name of the MQTT server.
 * @param tagEventTopic - The topic for tag event.
 * @param clientId - The client ID for MQTT connection.
 * @param userName - The username for MQTT authentication.
 * @param password - The password for MQTT authentication.
 * @param port - The port number for MQTT connection.
 * @returns A promise that resolves to the response of the MQTT configuration update.
 */
export async function updateMqttConfigurationRequest(
    ip: string,
    enable: boolean,
    hostName: string,
    tagEventTopic: string,
    clientId: string,
    userName: string,
    password: string,
    port: string
): Promise<UpdateMqttConfigurationResponse> {
    const url = `http://${ip}:8080/MqttConfigurationController/updateMqttConfigurationRequest`

    const data = {
        type: "Reader-updateMqttConfigurationRequest",
        enable: enable.toString(),
        normalConfiguration: {
            enable: enable.toString(),
            normalParameters: {
                hostName,
                topics: {tagEventTopic},
                clientId,
                userName,
                password,
                port
            }
        }
    }

    const response = await sendPostRequest(url, data)
    return response as UpdateMqttConfigurationResponse
}

/**
 * Sends a query temperature request to the specified IP address.
 * @param ip - The IP address of the temperature controller.
 * @returns A promise that resolves to a QueryTemperatureResponse object.
 */
export async function queryTemperatureRequest(ip: string): Promise<QueryTemperatureResponse> {
    const url = `http://${ip}:8080/TemperatureController/queryTemperatureRequest`

    const data = {
        type: "Reader-queryTemperatureRequest",
        functionList: [
            {name: "uhfModuleTemperature"}
        ]
    }

    const response = await sendPostRequest(url, data)
    return response as QueryTemperatureResponse
}
