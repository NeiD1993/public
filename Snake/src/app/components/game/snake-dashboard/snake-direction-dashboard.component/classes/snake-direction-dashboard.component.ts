import { ActivatableGameComponentViewState } from "../../../base.game.component/base-activatable-game.component/base-activatable-game.component.view-state";
import { AnimationService } from "src/app/services/animation.service";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { BaseGameStateChangedComponent } from "../../../base.game.component/base-game-state.component/base-game-state-changed.component";
import { BonusType } from "src/app/enums/bonus-type";
import { ChangeDetectorRef, Component, forwardRef, Inject } from "@angular/core";
import { createSubscriptionServiceFactoryProvider, SubscriptionsService } from "src/app/services/subscriptions.service";
import { Direction } from "src/app/enums/direction";
import { GameEventsSubscriptionsServiceToken } from "../../../base.game.component/base-activatable-game.component/base-activatable-game.component";
import { GameEventType } from "src/app/enums/game/game-event-type";
import { SettingsService } from "src/app/services/settings.service";
import { snakeDirectionDashboardComponentAnimations } from "./snake-direction-dashboard.component.animations";
import { SnakeDirectionDashboardComponentViewState } from "./snake-direction-dashboard.component.view-state";
import { StandardBrick } from "src/app/models/brick/standard-brick";

@Component({
    selector: 'snake-direction-dashboard-root',
    styleUrls: ['../../snake-dashboard.component.css', '../snake-direction-dashboard.component.css'],
    templateUrl: '../snake-direction-dashboard.component.html',
    animations: [snakeDirectionDashboardComponentAnimations],
    providers: [
        { provide: BaseGameStateChangedComponent, useExisting: forwardRef(() => SnakeDirectionDashboardComponent) },
        createSubscriptionServiceFactoryProvider<GameEventType>(GameEventsSubscriptionsServiceToken)
    ]
})
export class SnakeDirectionDashboardComponent extends BaseGameStateChangedComponent {

    constructor(changeDetector: ChangeDetectorRef, gameLogicService: BaseGameLogicService,
        @Inject(GameEventsSubscriptionsServiceToken) gameEventsSubscriptionsService: SubscriptionsService<GameEventType>,
        private _animationService: AnimationService, public settingsService: SettingsService) {
        super(changeDetector, gameLogicService, gameEventsSubscriptionsService);
        this._gameEventsSubscriptionsService.addSubscription(GameEventType.SnakeDirectionChanged,
            this.gameLogicService.snakeDirectionChanged.subscribe(this.onSnakeDirectionChanged.bind(this)));
        this._gameEventsSubscriptionsService.addSubscription(GameEventType.SnakeSpeedLevelChanged,
            this.gameLogicService.snakeSpeedLevelChanged.subscribe(this.onSnakeSpeedLevelChanged.bind(this)));
    }

    protected createViewState(): ActivatableGameComponentViewState {
        return new SnakeDirectionDashboardComponentViewState();
    }

    protected getGameStartedAnimationPerformedValue(animatedElement: any) {
        return this;
    }

    ngOnInit(): void {
        super.ngOnInit();
        (<SnakeDirectionDashboardComponentViewState>this.viewState).createAnimationFactories(this._animationService);
    }

    onFoodEaten(foodBrick: StandardBrick): void {
        this.update();
    }

    onSnakeDirectionChanged(direction: Direction): void {
        this.update();
    }

    onSnakeSpeedLevelChanged(bonusType: BonusType): void {
        (<SnakeDirectionDashboardComponentViewState>this.viewState).setSpeedLevelTextChangedAnimation(bonusType);
        this.update();
    }
}