import { DependencyContainer } from "tsyringe";

import { CustomInventoryItemEventRouter } from "./CustomItemEventRouter";
import { IPreSptLoadMod } from "@spt/models/external/IPreSptLoadMod";

class Mod implements IPreSptLoadMod {
    preSptLoad(container: DependencyContainer): void {
        container.registerType("IERouters", "CustomInventoryItemEventRouter");

        container.register<CustomInventoryItemEventRouter>("CustomInventoryItemEventRouter", {
            useClass: CustomInventoryItemEventRouter
        })
    }

}

export const mod = new Mod();
