import CreateTrayForm from "@/components/tray/create-tray-form"
import {Separator} from "@/components/ui/separator"

export default function CreateTray() {
    return (
        <div className="space-y-6">
            <div>
                <h3 className="text-lg font-medium">Create New Tray</h3>
                <p className="text-sm text-muted-foreground">
                    First hold the tray in front of the antenna to get EPC. Afterwrds, key in Rack ID.
                </p>
            </div>
            <Separator/>
            <CreateTrayForm/>
        </div>
    )
}