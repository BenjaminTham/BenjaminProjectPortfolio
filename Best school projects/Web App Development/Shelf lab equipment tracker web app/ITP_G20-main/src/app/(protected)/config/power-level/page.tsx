import {Separator} from "@/components/ui/separator"
import {PowerLevelForm} from "@/components/power-level/profile-form";

export default function PowerLevel() {
    return (
        <div className="space-y-6">
            <div>
                <h3 className="text-lg font-medium">Power Level</h3>
                <p className="text-sm text-muted-foreground">
                    Manage your power level settings here.
                </p>
            </div>
            <Separator/>
            <PowerLevelForm/>
        </div>
    )
}