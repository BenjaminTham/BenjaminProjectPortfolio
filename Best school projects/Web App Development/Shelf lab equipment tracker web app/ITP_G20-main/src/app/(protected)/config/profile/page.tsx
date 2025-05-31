import {ProfileForm} from "@/components/profile/profile-form"
import {Separator} from "@/components/ui/separator"

export default function Profile() {
    return (
        <div className="space-y-6">
            <div>
                <h3 className="text-lg font-medium">Profile</h3>
                <p className="text-sm text-muted-foreground">
                    Manage your account settings here.
                </p>
            </div>
            <Separator/>
            <ProfileForm/>
        </div>
    )
}