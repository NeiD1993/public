import { ActivatableGameComponentViewState } from "../../../base.game.component/base-activatable-game.component/base-activatable-game.component.view-state";
import { AnimationService } from "src/app/services/animation.service";
import { BaseGameStateChangedComponent } from "../../../base.game.component/base-game-state.component/base-game-state-changed.component";
import { BaseGameStateFoodStatusChangedComponent } from "../../../base.game.component/base-game-state.component/base-game-state-food-status-changed.component";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { Component, ChangeDetectorRef, forwardRef, Inject } from "@angular/core";
import { GameEventsSubscriptionsServiceToken } from "../../../base.game.component/base-activatable-game.component/base-activatable-game.component";
import { GameEventType } from "src/app/enums/game/game-event-type";
import { snakeLengthDashboardComponentAnimations } from "./snake-length-dashboard.component.animations";
import { SnakeLengthDashboardComponentViewState } from "./snake-length-dashboard.component.view-state";
import { StandardBrick } from "src/app/models/brick/standard-brick";
import { createSubscriptionServiceFactoryProvider, SubscriptionsService } from "src/app/services/subscriptions.service";

@Component({
    selector: 'snake-length-dashboard-root',
    styleUrls: ['../../snake-dashboard.component.css', '../snake-length-dashboard.component.css'],
    templateUrl: '../snake-length-dashboard.component.html',
    animations: [snakeLengthDashboardComponentAnimations],
    providers: [
        { provide: BaseGameStateChangedComponent, useExisting: forwardRef(() => SnakeLengthDashboardComponent) },
        createSubscriptionServiceFactoryProvider<GameEventType>(GameEventsSubscriptionsServiceToken),
    ]
})
export class SnakeLengthDashboardComponent extends BaseGameStateFoodStatusChangedComponent {

    constructor(changeDetector: ChangeDetectorRef, gameLogicService: BaseGameLogicService,
        @Inject(GameEventsSubscriptionsServiceToken) gameEventsSubscriptionsService: SubscriptionsService<GameEventType>,
        private _animationService: AnimationService) {
        super(changeDetector, gameLogicService, gameEventsSubscriptionsService);
    }

    protected createViewState(): ActivatableGameComponentViewState {
        return new SnakeLengthDashboardComponentViewState();
    }

    protected getGameStartedAnimationPerformedValue(animatedElement: any) {
        return this;
    }

    ngOnInit(): void {
        super.ngOnInit();
        (<SnakeLengthDashboardComponentViewState>this.viewState).createAnimationFactories(this._animationService);
    }

    onFoodEaten(foodBrick: StandardBrick): void {
        this.update();
    }

    onNewFoodGenerated(foodBrick: StandardBrick): void {
        this.update();
    }
}