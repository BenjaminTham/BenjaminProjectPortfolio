import {CreateReaderForm} from "@/components/reader/create-reader-form";
import {ReaderSetting} from "@/components/reader/reader-setting";
import {Separator} from "@/components/ui/separator";

export default function Reader() {
    return (
        <div className="space-y-6">
            <div>
                <h3 className="text-lg font-medium">Reader</h3>
                <p className="text-sm text-muted-foreground">
                    Manage the UR8A reader settings here.
                </p>
            </div>
            <Separator/>
            <CreateReaderForm/>
            <Separator/>
            <ReaderSetting/>
        </div>
    );
}
