import {Label} from "@/components/ui/label";
import {ModeToggle} from "@/components/appearance/mode-toggle";

export function AppearanceSettings() {
    return (
        <>
            <div className="space-y-8">
                <Label>Theme</Label>
                <ModeToggle/>
            </div>
        </>
    )
}