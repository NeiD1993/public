import { $enum } from "ts-enum-util";
import { ActivatableGameComponentViewState } from "../../base.game.component/base-activatable-game.component/base-activatable-game.component.view-state";
import { ActivatedRoute, Router } from "@angular/router";
import { AnimationService } from "src/app/services/animation.service";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { BaseGameStateChangedComponent } from "../../base.game.component/base-game-state.component/base-game-state-changed.component";
import { ChangeDetectorRef, Component, Inject, InjectionToken, QueryList, ViewChild, ViewChildren } from "@angular/core";
import { ComponentState } from "src/app/enums/component-state";
import { Difficulty } from "src/app/enums/difficulty";
import { FieldComponent } from "../../field/field.component/classes/field.component";
import { gameComponentAnimations } from "./game.component.animations";
import { GameComponentViewState } from "./game.component.view-state";
import { GameEventsSubscriptionsServiceToken } from "../../base.game.component/base-activatable-game.component/base-activatable-game.component";
import { GameEventType } from "src/app/enums/game/game-event-type";
import { GameLogicService } from "src/app/services/game-logic/game-logic.service";
import { GameState } from "src/app/enums/game/game-state";
import { LoadingComponent } from "../../loading.component/classes/loading.component";
import { SubscriptionsService, createSubscriptionServiceFactoryProvider } from "src/app/services/subscriptions.service";

let QueryParamsEventsSubscriptionsServiceToken: InjectionToken<Router> = new InjectionToken<Router>('QueryParamsEventsSubscriptionsService');

@Component({
    selector: 'game-root',
    styleUrls: ['../../game.css', '../game.component.css'],
    templateUrl: '../game.component.html',
    providers: [
        { provide: BaseGameLogicService, useClass: GameLogicService },
        createSubscriptionServiceFactoryProvider<GameEventType>(GameEventsSubscriptionsServiceToken),
        createSubscriptionServiceFactoryProvider<Router>(QueryParamsEventsSubscriptionsServiceToken),
    ],
    animations: [gameComponentAnimations]
})
export class GameComponent extends BaseGameStateChangedComponent {

    private _isPrepared: boolean;

    constructor(activatedRoute: ActivatedRoute, changeDetector: ChangeDetectorRef, gameLogicService: BaseGameLogicService,
        @Inject(GameEventsSubscriptionsServiceToken) gameEventsSubscriptionsService: SubscriptionsService<GameEventType>,
        @Inject(QueryParamsEventsSubscriptionsServiceToken)
        private _queryParamsEventsSubscriptionsService: SubscriptionsService<Router>,
        private _animationService: AnimationService, private _router: Router) {
        super(changeDetector, gameLogicService, gameEventsSubscriptionsService);
        this._queryParamsEventsSubscriptionsService.addSubscription(this._router,
            activatedRoute.queryParams.subscribe((queryParams: any) => {
                let difficulty: Difficulty = $enum(Difficulty).getValueOrDefault(queryParams['difficulty']);

                if (difficulty !== undefined)
                    this.gameLogicService.difficulty = difficulty;
                else
                    this.navigateToMain();
            })
        );
    }

    get fieldComponent(): FieldComponent {
        return <FieldComponent>(this.gameChildComponents.find(gameChildComponent => gameChildComponent instanceof FieldComponent));
    }

    get gameComponents(): Array<BaseGameStateChangedComponent> {
        return this.gameChildComponents.toArray().concat(this);
    }

    @ViewChildren(BaseGameStateChangedComponent)
    gameChildComponents: QueryList<BaseGameStateChangedComponent>;

    get isPrepared(): boolean {
        return this._isPrepared;
    }

    @ViewChild(LoadingComponent)
    loadingComponent: LoadingComponent;

    private updateGameChildComponents(): void {
        this.gameChildComponents.forEach(gameChildComponent => gameChildComponent.update());
    }

    private displayComponents(): void {
        this.gameComponents.forEach(gameComponent => {
            gameComponent.display();
            gameComponent.update();
        });
    }

    private hideAndDeactivateComponents(): void {
        if ((this.gameLogicService.gameState == GameState.NotInitialized) || (this.gameLogicService.gameState == GameState.Finished))
            this.hide();
        else {
            this.deactivate();
            this.gameChildComponents.forEach(gameComponent => {
                if (gameComponent instanceof FieldComponent)
                    gameComponent.hide();
                else
                    gameComponent.deactivate();

                gameComponent.update();
            });
        }

        this.update();
    }

    private navigateToMain(): void {
        this._router.navigateByUrl('main');
    }

    protected createViewState(): ActivatableGameComponentViewState {
        return new GameComponentViewState();
    }

    protected getGameStartedAnimationPerformedValue(animatedElement: any): any {
        return animatedElement;
    }

    changeGameState(): void {
        switch (this.gameLogicService.gameState) {
            case GameState.NotStarted:
                this.gameLogicService.startGame();
                break;
            case GameState.Started:
                this.gameLogicService.pauseGame();
                break;
            default:
                this.gameLogicService.startGame();
        }
    }

    endGame(): void {
        this.navigateToMain();
    }

    initializeGame(reinitialize: boolean): void {
        let gameInitializationResult: boolean;

        this._isPrepared = false;

        if (reinitialize)
            this.hideAndDeactivateComponents();

        gameInitializationResult = this.gameLogicService.initializeGame(reinitialize);

        if (gameInitializationResult) {
            (<GameComponentViewState>this.viewState).createAnimationFactories(this._animationService);

            if (this.state == ComponentState.NotDisplayed)
                this.updateGameChildComponents();

            this.loadingComponent.update();
        }
        else {
            if (this.gameLogicService.gameInitializationAttemptsCountRest == 0)
                this.navigateToMain();
            else {
                this.display();
                this.update();
            }
        }
    }

    restartGame(): void {
        let fieldComponent: FieldComponent = this.fieldComponent;

        if (fieldComponent)
            fieldComponent.prepareToReset();

        this.initializeGame(true);
    }

    ngOnDestroy(): void {
        super.ngOnDestroy();
        this._gameEventsSubscriptionsService.resetSubscriptions();
    }

    ngOnInit(): void {
        super.ngOnInit();
        this.initializeGame(false);
    }

    onFieldRendered(): void {
        this._isPrepared = true;
        this.update();
    }

    onLoadingComponentDestroying(): void {
        this.displayComponents();
    }

    onLoadingProgressChanged(loadingPosition: number): void {
        this.loadingComponent.updatePosition(loadingPosition);
    }
}