import {Separator} from "@/components/ui/separator"
import {SidebarNav} from "@/components/shared/sidebar-nav"
import {trayItems} from "@/config/docs"

interface ConfigLayoutProps {
    children: React.ReactNode
}

export default function ConfigManagementLayout({children}: ConfigLayoutProps) {
    return (
        <>
            <div className="space-y-6 p-10 pb-16 block">
                <div className="space-y-0.5">
                    <h2 className="text-2xl font-bold tracking-tight">Trays</h2>
                    <p className="text-muted-foreground">
                        Manage your trays here.
                    </p>
                </div>
                <Separator className="my-6"/>
                <div className="flex flex-col space-y-8 lg:flex-row lg:space-x-12 lg:space-y-0">
                    <aside className="-mx-4 lg:w-1/5">
                        <SidebarNav items={trayItems}/>
                    </aside>
                    <div className="flex-1">{children}</div>
                </div>
            </div>
        </>
    )
}