import CreateTrayForm from "@/components/tray/read-tray"
import {Separator} from "@/components/ui/separator"

export default function ReadTray() {
    return (
        <div className="space-y-6">
            <div>
                <h3 className="text-lg font-medium">Read Tray</h3>
                <p className="text-sm text-muted-foreground">
                    Type in the relevant tray details. Hold the tray in front of the antenna and press scan
                </p>
            </div>
            <Separator/>
            <CreateTrayForm/>
        </div>
    )
}