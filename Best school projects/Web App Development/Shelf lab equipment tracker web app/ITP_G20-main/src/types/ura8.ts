//src/types/ura8.ts

interface BaseResponse {
    type: string;
    message: string;
}

interface StatusCode {
    code: number;
    message: string;
}

interface AntennaPower extends StatusCode {
    antennaPort: number;
    power: number;
}

export interface QueryAntennaPowerResponse extends BaseResponse {
    antennaPower: AntennaPower[];
}

interface AntennaEnabledState extends StatusCode {
    antennaPort: number;
    enable: boolean;
}

export interface QueryAntennaEnabledStateResponse extends BaseResponse {
    antennaEnabledState: AntennaEnabledState[];
}

interface Frequency extends StatusCode {
    region: number;
}

export interface QueryFrequencyResponse extends BaseResponse {
    frequency: Frequency;
}

interface InventorySearchMode extends StatusCode {
    queryTarget: string;
    querySession: string;
}

export interface QueryInventorySearchModeResponse extends BaseResponse {
    inventorySearchMode: InventorySearchMode;
}

interface InventoryMemoryBank extends StatusCode {
    memoryBank: string;
    wordUserOffset: number;
    wordUserLength: number;
}

export interface QueryInventoryMemoryBankResponse extends BaseResponse {
    inventoryMemoryBank: InventoryMemoryBank;
}

interface TagReporting {
    duplicateTagFilter: {
        enable: boolean;
        filterByTime: number;
    },
    rssiFilter: {
        enable: boolean;
        rssiIsGreateThan: number;
    },
    tagReportingMode: {
        mode: string;
    }
}

export interface QueryTagReportingResponse extends BaseResponse {
    tagReporting: TagReporting;
}

interface HeartbeatPacket extends StatusCode {
    enable: boolean;
    intervalTime: number;
}

export interface QueryHeartbeatPacketResponse extends BaseResponse {
    heartbeatPacket: HeartbeatPacket;
}

interface BuzzerForInventory {
    enable: boolean;
}

export interface QueryBuzzerForInventoryResponse extends BaseResponse {
    buzzerForInventory: BuzzerForInventory;
}

interface Tagfocus extends StatusCode {
    enable: boolean;
}

export interface QueryTagfocusResponse extends BaseResponse {
    tagfocus: Tagfocus;
}

interface FastID extends StatusCode {
    enable: boolean;
}

export interface QueryFastIDResponse extends BaseResponse {
    fastID: FastID;
}

interface LinkFrequency extends StatusCode {
    value: string;
}

export interface QueryLinkFrequencyResponse extends BaseResponse {
    linkFrequency: LinkFrequency;
}

interface AfterNetworkDisconnected {
    code: number;
    message: string;
    cacheTags: boolean;
}

export interface QueryAfterNetworkDisconnectedResponse extends BaseResponse {
    afterNetworkDisconnected: AfterNetworkDisconnected;
}

export interface UpdateAntennaPowerResponse extends BaseResponse {
    antennaPower: StatusCode;
}

export interface UpdateAntennaEnabledStateResponse extends BaseResponse {
    antennaEnabledState: StatusCode;
}

export interface UpdateFrequencyResponse extends BaseResponse {
    frequency: StatusCode;
}

export interface UpdateInventorySearchModeResponse extends BaseResponse {
    inventorySearchMode: StatusCode;
}

export interface UpdateInventoryMemoryBankResponse extends BaseResponse {
    inventoryMemoryBank: StatusCode;
}

export interface UpdateTagReportingResponse extends BaseResponse {
    tagReporting: {
        duplicateTagFilter: StatusCode;
        rssiFilter: StatusCode;
        tagReportingMode: StatusCode;
    }
}

export interface UpdateHeartbeatPacketResponse extends BaseResponse {
    heartbeatPacket: StatusCode;
}

export interface UpdateBuzzerForInventoryResponse extends BaseResponse {
    buzzerForInventory: StatusCode;
}

export interface UpdateTagfocusResponse extends BaseResponse {
    tagfocus: StatusCode;
}

export interface UpdateFastIDResponse extends BaseResponse {
    fastID: StatusCode;
}

export interface UpdateLinkFrequencyResponse extends BaseResponse {
    linkFrequency: StatusCode;
}

export interface UpdateAfterNetworkDisconnectedResponse extends BaseResponse {
    afterNetworkDisconnected: StatusCode;
}

export interface TagReadReponse extends StatusCode, BaseResponse {
    hexTagData: string;
}

export interface TagWriteResponse extends StatusCode, BaseResponse {
}

export interface UpdateMqttConfigurationResponse extends BaseResponse {
    normalConfiguration: StatusCode;
}

export interface queryMqttConfigurationResponse extends BaseResponse, StatusCode {
    enable: boolean;
    normalConfiguration:
        {
            enable: boolean;
            normalParameters:
                {
                    hostName: string;
                    topics: {
                        tagEventTopic: string;
                    };
                    clientId: string;
                    port: number;
                }
        }
}


interface Temperature extends StatusCode {
    temperature: number;
}

export interface QueryTemperatureResponse extends BaseResponse {
    uhfModuleTemperature: Temperature;
}

export interface StartInventoryResponse extends StatusCode, BaseResponse {
}

export interface TagReportingData {
    pcHex: string;
    epcHex: string;
    rssi: number;
    count: number;
    index: number;
    antennaPort: number;
    isNewTag: boolean;
}

export interface TagReportingDataAndIndexResponse {
    type: string;
    data: TagReportingData[];
}

export interface StopInventoryResponse extends StatusCode, BaseResponse {
}

