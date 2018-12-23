import { $enum, EnumWrapper } from "ts-enum-util";
import { ActivatableGameComponentViewState } from "../../../base.game.component/base-activatable-game.component/base-activatable-game.component.view-state";
import { AnimationEvent } from "@angular/animations";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { BaseGameStateChangedComponent } from "../../../base.game.component/base-game-state.component/base-game-state-changed.component";
import { BaseGameStateFoodStatusChangedComponent } from "../../../base.game.component/base-game-state.component/base-game-state-food-status-changed.component";
import { BrickComponent } from "../../brick.component/classes/brick.component";
import { ChangeDetectorRef, Component, EventEmitter, forwardRef, HostListener, Inject, InjectionToken, Output } from "@angular/core";
import { createSubscriptionServiceFactoryProvider, SubscriptionsService } from "src/app/services/subscriptions.service";
import { Direction } from "src/app/enums/direction";
import { fieldComponentAnimations } from "./field.component.animations";
import { FieldComponentViewState } from "./field.component.view-state";
import { FoodStatus } from "src/app/enums/food-status";
import { GameEventsSubscriptionsServiceToken } from "../../../base.game.component/base-activatable-game.component/base-activatable-game.component";
import { GameEventType } from "src/app/enums/game/game-event-type";
import { GameState } from "src/app/enums/game/game-state";
import { IBrickDecorationService, IBrickDecorationServiceToken } from "src/app/services/brick-decoration/i-brick-decoration.service";
import { StandardBrick } from "src/app/models/brick/standard-brick";

let BrickComponentsMovingAnimationPerformedEventSubscriptionsServiceToken: InjectionToken<any> =
    new InjectionToken('BrickComponentsMovingAnimationPerformedEventSubscriptionsService');

@Component({
    selector: 'field-root',
    styleUrls: ['../../../game.css', '../field.component.css'],
    templateUrl: '../field.component.html',
    animations: [fieldComponentAnimations],
    providers: [
        { provide: BaseGameStateChangedComponent, useExisting: forwardRef(() => FieldComponent) },
        createSubscriptionServiceFactoryProvider<GameEventType>(GameEventsSubscriptionsServiceToken),
        createSubscriptionServiceFactoryProvider<BrickComponent>(BrickComponentsMovingAnimationPerformedEventSubscriptionsServiceToken)
    ]
})
export class FieldComponent extends BaseGameStateFoodStatusChangedComponent {

    constructor(changeDetector: ChangeDetectorRef, gameLogicService: BaseGameLogicService,
        @Inject(GameEventsSubscriptionsServiceToken) gameEventsSubscriptionsService: SubscriptionsService<GameEventType>,
        @Inject(IBrickDecorationServiceToken) private _bricksDecorationService: IBrickDecorationService,
        @Inject(BrickComponentsMovingAnimationPerformedEventSubscriptionsServiceToken)
        private _brickComponentsMovingAnimationPerformedEventSubscriptionsService: SubscriptionsService<BrickComponent>) {
        super(changeDetector, gameLogicService, gameEventsSubscriptionsService);
        this._gameEventsSubscriptionsService.addSubscription(GameEventType.SnakePositionChanged,
            this.gameLogicService.snakePositionChanged.subscribe(this.onSnakePositionChanged.bind(this)));
    }

    brickComponents: Array<BrickComponent>;

    private detectChangesInBricks(bricks: Array<StandardBrick>) {
        bricks.forEach(brick => this.getBrickComponent(brick).update());
    }

    private getBrickComponent(brick: StandardBrick): BrickComponent {
        return this.brickComponents[this.gameLogicService.field.bricks.getBrickIndex(brick)];
    }

    private tryMoveSnake(): void {
        if (this._brickComponentsMovingAnimationPerformedEventSubscriptionsService.subscriptions.size == 0)
            this.gameLogicService.moveSnake();
    }

    protected createViewState(): ActivatableGameComponentViewState {
        let viewState: FieldComponentViewState = new FieldComponentViewState(this._bricksDecorationService, this.gameLogicService.difficulty,
            this.gameLogicService.field.bricks.sideBricksCount);

        return viewState;
    }

    protected getGameStartedAnimationPerformedValue(animatedElement: any): any {
        return this;
    }

    ngOnDestroy(): void {
        super.ngOnDestroy();
        this.prepareToReset();
    }

    onBrickComponentMovingAnimationPerformed(brickComponent: BrickComponent): void {
        this._brickComponentsMovingAnimationPerformedEventSubscriptionsService.tryRemoveSubscription(brickComponent);
        this.tryMoveSnake();
    }

    onFieldIntervalForeachExecutionComplete(embeddedBrickComponents: Array<BrickComponent>) {
        this.brickComponents = embeddedBrickComponents;
        this.update();
        this.fieldRendered.emit();
    }

    onFieldIntervalForeachPositionChanged(foreachPosition: number): void {
        this.update();
        this.loadingProgressChanged.emit(foreachPosition);
        this.loadingContinued.emit();
    }

    onFoodEaten(foodBrick: StandardBrick): void {
        this.detectChangesInBricks([foodBrick]);
        this.update();
    }

    onFoodStatusTriggerDone(animationEvent: AnimationEvent): void {
        let foodStatusWrapper: EnumWrapper = $enum(FoodStatus);

        if ((foodStatusWrapper.getValueOrDefault(animationEvent.fromState) == FoodStatus.Fresh) &&
            (foodStatusWrapper.getValueOrDefault(animationEvent.toState) == FoodStatus.Eaten))
            this.gameLogicService.moveSnake();
    }

    onGameStateTriggerDone(animationEvent: AnimationEvent): void {
        if ($enum(GameState).getValueOrDefault(animationEvent.toState) == GameState.Started)
            this.tryMoveSnake();
    }

    @HostListener("document:keydown", ['$event'])
    onKeyUp(keyboardEvent: KeyboardEvent) {
        switch (keyboardEvent.key) {
            case "ArrowDown":
                this.gameLogicService.changeSnakeDirection(Direction.Down);
                break;
            case "ArrowLeft":
                this.gameLogicService.changeSnakeDirection(Direction.Left);
                break;
            case "ArrowRight":
                this.gameLogicService.changeSnakeDirection(Direction.Right);
                break;
            case "ArrowUp":
                this.gameLogicService.changeSnakeDirection(Direction.Up);
                break;
        }
    }

    onNewFoodGenerated(foodBrick: StandardBrick): void {
        this.detectChangesInBricks([foodBrick]);
        this.update();
    }

    onSnakePositionChanged(updatedSnakeEndBricks: Array<StandardBrick>): void {
        updatedSnakeEndBricks.forEach(updatedSnakeEndBrick => {
            let updatedSnakeEndBrickComponent: BrickComponent = this.getBrickComponent(updatedSnakeEndBrick);

            this._brickComponentsMovingAnimationPerformedEventSubscriptionsService.addSubscription(updatedSnakeEndBrickComponent,
                updatedSnakeEndBrickComponent.movingAnimationPerformed.subscribe(this.onBrickComponentMovingAnimationPerformed.bind(this,
                    updatedSnakeEndBrickComponent))
            );
        });
        this.detectChangesInBricks(updatedSnakeEndBricks);
    }

    prepareToReset(): void {
        this._brickComponentsMovingAnimationPerformedEventSubscriptionsService.resetSubscriptions();
    }

    @Output()
    fieldRendered: EventEmitter<any> = new EventEmitter();

    @Output()
    loadingContinued: EventEmitter<any> = new EventEmitter();

    @Output()
    loadingProgressChanged: EventEmitter<number> = new EventEmitter<number>();
}