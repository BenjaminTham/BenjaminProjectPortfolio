"use client";

import {useState} from "react";
import {Button} from "@/components/ui/button";
import {Input} from "@/components/ui/input";
import {toast} from "@/components/ui/use-toast";
import {readTray} from "@/domain/trayControl";
import {Tray} from "@prisma/client";
import {Label} from "@/components/ui/label";

export default function ReadTray() {
    const [tray, setTray] = useState<Tray>();
    const [isScanDisabled, setScanDisabled] = useState(false);

    async function handleScan() {
        setScanDisabled(true);
        const trayData = await readTray();
        setScanDisabled(false);

        // If the trayData a tray object, set the tray state to the tray object
        if (trayData && "id" in trayData) {
            setTray(trayData);
        } else {
            toast({
                title: trayData.error,
                variant: "destructive",
            });
            setTray(undefined);
        }
    }

    return (
        <div className="space-y-4">
            <div className="space-y-4">
                <Label>Tray ID</Label>
                <Input disabled className="w-96" placeholder="Tray ID" value={tray?.id || ""} />
            </div>
            <div className="space-y-4">
                <Label>Tray EPC</Label>
                <Input disabled className="w-96" placeholder="Tray EPC" value={tray?.epc || ""} />
            </div>
            <div className="space-y-4">
                <Label>Status</Label>
                <Input disabled className="w-96" placeholder="Status" value={tray?.status || ""} />
            </div>
            <div className="space-y-4">
                <Label>In/Out Time</Label>
                <Input disabled className="w-96" placeholder="In/Out Time" value={tray?.statusChg.toLocaleString() || ""} />
            </div>
            <div className="space-y-4">
                <Label>Rack ID</Label>
                <Input disabled className="w-96" placeholder="Rack ID" value={tray?.rackId || ""} />
            </div>
            <Button type="button" onClick={handleScan} disabled={isScanDisabled}>
                Scan
            </Button>
        </div>
    );
}