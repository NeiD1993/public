import { BaseGameStateChangedComponent } from "./base-game-state-changed.component";
import { GameEventType } from "src/app/enums/game/game-event-type";
import { Subscription } from "rxjs";
import { StandardBrick } from "src/app/models/brick/standard-brick";

export abstract class BaseGameStateFoodStatusChangedComponent extends BaseGameStateChangedComponent {

    protected generateGameEventsSubscriptions(): Map<GameEventType, Subscription> {
        let gameEventsSubsriptions: Map<GameEventType, Subscription> = super.generateGameEventsSubscriptions();
        
        gameEventsSubsriptions.set(GameEventType.FoodEaten, this.gameLogicService.foodEaten.subscribe(this.onFoodEaten.bind(this)));
        gameEventsSubsriptions.set(GameEventType.NewFoodGenerated, this.gameLogicService.newFoodGenerated.subscribe(this.onNewFoodGenerated.bind(this)));

        return gameEventsSubsriptions;
    }

    abstract onFoodEaten(foodBrick: StandardBrick): void;

    abstract onNewFoodGenerated(foodBrick: StandardBrick): void;
}