import { inject, injectable } from "tsyringe";

import { HandledRoute, ItemEventRouterDefinition } from "@spt/di/Router"
import { IPmcData } from "@spt/models/eft/common/IPmcData";
import { IItemEventRouterResponse } from "@spt/models/eft/itemEvent/IItemEventRouterResponse";
import { EventOutputHolder } from "@spt/routers/EventOutputHolder";
import { InventoryHelper } from "@spt/helpers/InventoryHelper";
import { ILogger } from "@spt/models/spt/utils/ILogger";

@injectable()
export class CustomInventoryItemEventRouter extends ItemEventRouterDefinition {
    constructor(
        @inject("WinstonLogger") protected logger: ILogger,
        @inject("EventOutputHolder") protected eventOutputHolder: EventOutputHolder,
        @inject("InventoryHelper") protected inventoryHelper: InventoryHelper,
    ) {
        super();
    }

    public override getHandledRoutes(): HandledRoute[] {
        return [
            new HandledRoute("Combine", false)
        ];
    }

    public override async handleItemEvent(
        url: string,
        pmcData: IPmcData,
        body: any,
        sessionID: string,
        output: IItemEventRouterResponse,
    ): Promise<IItemEventRouterResponse> {
        switch (url) {
            case "Combine":
                output = this.eventOutputHolder.getOutput(sessionID);
                this.HandleCombine(pmcData, body.sourceItem, body.targetItem, body.sourceAmount, body.targetAmount, body.type, sessionID, output);
                return output;
        }
        return output;
    }

    private HandleCombine(pmcData: IPmcData, sourceItem: string, targetItem: string, sourceAmount: number, targetAmount: number, type: string, sessionID: string, output: IItemEventRouterResponse) {
        const sItem = pmcData.Inventory.items.find((item) => item._id === sourceItem);
        const tItem = pmcData.Inventory.items.find((item) => item._id === targetItem);

        if (!sItem || !tItem) {
            this.logger.error(`Could not find source or target item! Source: ${sourceItem}, Target: ${targetItem}`);
            return;
        }

        switch (type) {
            case "medical":
                if (sItem.upd?.MedKit && tItem.upd?.MedKit) {
                    sItem.upd.MedKit.HpResource = sourceAmount;
                    tItem.upd.MedKit.HpResource = targetAmount;
                }
                else {
                    this.logger.error("MedKit was missing on source or target item!");
                    return;
                }
                break;
            case "food":
                if (sItem.upd?.FoodDrink && tItem?.upd.FoodDrink) {
                    sItem.upd.FoodDrink.HpPercent = sourceAmount;
                    tItem.upd.FoodDrink.HpPercent = targetAmount;
                }
                else {
                    this.logger.error("FoodDrink was missing on source or target item!");
                    return;
                }
                break;
            default:
                this.logger.warning(`Unknown type: ${type}`);
                break;
        }

        if (sourceAmount == 0) {
            this.inventoryHelper.removeItem(pmcData, sourceItem, sessionID, output);
        }
    }
}