import {HostIpForm} from "@/components/host-ip/host-ip-form";
import {Separator} from "@/components/ui/separator";

export default function HostIpConfig() {
    return (
        <div className="space-y-6">
            <div>
                <h3 className="text-lg font-medium">Host IP Configuration</h3>
                <p className="text-sm text-muted-foreground">
                    Manage the host IP settings here.
                </p>
            </div>
            <Separator/>
            <HostIpForm/>
        </div>
    )
}