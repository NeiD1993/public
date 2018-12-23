import { BaseFieldBonusGeneratorService } from "../generators/entity/bonus/base-field-bonus-generator.service";
import { BaseFieldGeneratorService } from "../generators/field/base-field-generator.service";
import { BaseFoodGeneratorService } from "../generators/entity/food/base-food-generator.service";
import { BaseSnakeGeneratorService } from "../generators/entity/snake/base-snake-generator.service";
import { BonusType } from "src/app/enums/bonus-type";
import { BrickOperationsExecutorService } from "../brick-operations-executor.service";
import { BrickType } from "src/app/enums/brick-type";
import { Bricks } from "src/app/models/bricks";
import { Difficulty } from "src/app/enums/difficulty";
import { Direction } from "src/app/enums/direction";
import { FoodStatus } from "src/app/enums/food-status";
import { Field } from "src/app/models/field";
import { GameState } from "src/app/enums/game/game-state";
import { Injectable } from "@angular/core";
import { MathService } from "../math.service";
import { SettingsOptionType } from "src/app/enums/settings-option-type";
import { SettingsService } from "../settings.service";
import { Snake } from "src/app/models/snake/snake";
import { StandardBrick } from "src/app/models/brick/standard-brick";
import { Subject } from "rxjs";
import { TaskExecutor } from "../../tools/task-executor";

@Injectable()
export abstract class BaseGameLogicService {

    private static readonly _maxGameInitializationAttemptsCount: number = 5;

    private static readonly _movementStep: number = 1;

    private _difficulty: Difficulty;

    private _field: Field;

    private _foodCount: number;

    private _foodGeneratorTaskExecutor: TaskExecutor = new TaskExecutor();

    private _foodStatus: FoodStatus;

    private _gameInitializationAttemptsCountRest: number = BaseGameLogicService._maxGameInitializationAttemptsCount;

    private _gameState: GameState = GameState.NotInitialized;

    private _isFirstFoodEaten;

    constructor(private _brickOperationsExecutorService: BrickOperationsExecutorService,
        private _bonusGeneratorService: BaseFieldBonusGeneratorService, private _fieldGeneratorService: BaseFieldGeneratorService,
        private _foodGeneratorService: BaseFoodGeneratorService, private _snakeGeneratorService: BaseSnakeGeneratorService,
        protected _mathService: MathService, protected _settingsService: SettingsService) { }

    get gameInitializationAttemptsCountRest(): number {
        return this._gameInitializationAttemptsCountRest;
    }

    set difficulty(difficulty: Difficulty) {
        if (this.gameState == GameState.NotInitialized) {
            this._difficulty = difficulty;
            this._foodGeneratorTaskExecutor.delay = this.getFoodGenerationDelay();
        }
    }

    get difficulty(): Difficulty {
        return this._difficulty;
    }

    get field(): Field {
        return this._field;
    }

    get foodCount(): number {
        return this._foodCount;
    }

    set fieldGeneratorService(fieldGeneratorService: BaseFieldGeneratorService) {
        this._fieldGeneratorService = fieldGeneratorService;
    }

    set foodGeneratorService(foodGeneratorService: BaseFoodGeneratorService) {
        this._foodGeneratorService = foodGeneratorService;
    }

    get foodStatus(): FoodStatus {
        return this._foodStatus;
    }

    get foodStatusToString(): string {
        return FoodStatus[this._foodStatus];
    }

    get gameState(): GameState {
        return this._gameState;
    }

    get gameStateToString(): string {
        return GameState[this._gameState];
    }    

    get isFirstFoodEaten(): boolean {
        return this._isFirstFoodEaten;
    }

    set snakeGeneratorService(snakeGeneratorService: BaseSnakeGeneratorService) {
        this._snakeGeneratorService = snakeGeneratorService;
    }

    private changeBricks(changeMetadatas: Array<{ brick?: StandardBrick, enqueue: boolean }>, isReturnable: boolean): Array<StandardBrick> {
        let type: BrickType;

        changeMetadatas.forEach(changeMetadata => {
            if (changeMetadata.enqueue) {
                this.field.snake.bricks.enqueue(changeMetadata.brick);
                type = BrickType.Snake;
            }
            else {
                changeMetadata.brick = this.field.snake.bricks.dequeue();
                type = BrickType.Empty;
            }

            BaseFieldGeneratorService.changeBrick(this.field.bricks, changeMetadata.brick, type);
        });

        if (isReturnable)
            return changeMetadatas.map(changeMetadata => changeMetadata.brick);
    }

    private changeSnakeSpeed(bonusType: BonusType): void {
        switch (bonusType) {
            case BonusType.LevelDown:
                {
                    let decreasedSnakeSpeedLevel: number = this.field.snake.speedLevel - 1;

                    if (this.decreaseSnakeSpeedLevelCondition(decreasedSnakeSpeedLevel) &&
                        (this.field.snake.setSpeedLevel(decreasedSnakeSpeedLevel))) {
                        this.snakeSpeedLevelChanged.next(BonusType.LevelDown);
                    }
                }
                break;
            default:
                {
                    let increasedSnakeSpeedLevel: number = this.field.snake.speedLevel + 1;

                    if ((increasedSnakeSpeedLevel <= this._settingsService.getSettingsOption(SettingsOptionType.SnakeSpeedLevelsCount)) &&
                        this.increaseSnakeSpeedLevelCondition(increasedSnakeSpeedLevel)) {
                        this.field.snake.setSpeedLevel(increasedSnakeSpeedLevel);
                        this.snakeSpeedLevelChanged.next(BonusType.LevelUp);
                    }
                }
        }
    }

    private moveSnakeToEmptyBrick(followingSnakeHeadBrick: StandardBrick): void {
        let changedBricks: Array<StandardBrick> = this.changeBricks([{ enqueue: false }, { brick: followingSnakeHeadBrick, enqueue: true }], true);

        this.field.snake.forbiddenDirection = this.getForbiddenDirection();
        this.snakePositionChanged.next(changedBricks);
    }

    private moveSnakeToFoodBrick(followingSnakeHeadBrick: StandardBrick): void {
        this.changeBricks([{ brick: followingSnakeHeadBrick, enqueue: true }], false);
        this.field.snake.forbiddenDirection = this.getForbiddenDirection();
        this.changeSnakeSpeed(this._bonusGeneratorService.generateBonus());
        this._foodStatus = FoodStatus.Eaten;
        this._foodCount--;

        if (!this._isFirstFoodEaten)
            this._isFirstFoodEaten = true;

        this.foodEaten.next(followingSnakeHeadBrick);
        this.tryEndGame();
    }

    private resetGameParameters(): void {
        this._foodStatus = FoodStatus.Fresh;
        this._isFirstFoodEaten = false;
        this._foodCount = 0;
    }

    private tryEndGame(): void {
        if (this._foodCount == 0)
            this.endGame();
        else {
            this._foodGeneratorTaskExecutor.start(() => {
                let foodBrick: StandardBrick = this._foodGeneratorService.generateFood(this.field.bricks);

                this._foodStatus = FoodStatus.Fresh;
                this.newFoodGenerated.next(foodBrick);
            });
        }
    }

    protected abstract decreaseSnakeSpeedLevelCondition(decreasedSnakeSpeed: number): boolean;

    protected abstract getFoodGenerationDelay(): number;

    protected abstract getForbiddenDirection(): Direction;

    protected abstract increaseSnakeSpeedLevelCondition(increasedSnakeSpeed: number): boolean;

    changeSnakeDirection(direction: Direction): void {
        if ((this.gameState == GameState.Started) && (direction != this.field.snake.forbiddenDirection)) {
            this.field.snake.direction = direction;
            this.snakeDirectionChanged.next(direction);
        }
    }

    endGame(): void {
        if ((this.gameState == GameState.Started) || (this.gameState == GameState.Paused)) {
            this._foodGeneratorTaskExecutor.reset();
            this._gameState = GameState.Finished;
            this.gameStateChanged.next(GameState.Finished);
        }
    }

    initializeGame(reinitialize: boolean): boolean {
        let initializationResult: boolean = false;

        if (this._gameInitializationAttemptsCountRest > 0) {
            this._gameState = GameState.NotInitialized;
            this._foodGeneratorTaskExecutor.reset();
            this.resetGameParameters();

            if (!reinitialize || (reinitialize && !this._field))
                this._field = new Field(new Bricks(), new Snake());

            initializationResult = this._fieldGeneratorService.initializeField(this.difficulty, this._foodGeneratorService,
                this._snakeGeneratorService, this._field);

            if (initializationResult) {
                this._gameState = GameState.NotStarted;
                this._foodCount = this._mathService.getNumberPercentage(this.field.bricks.getTotalBricksCount() - this.field.snake.bricks.length,
                    (this._settingsService.getSettingsOption(SettingsOptionType.FieldFoodCountPercentage) / 100));
                this._gameInitializationAttemptsCountRest = BaseGameLogicService._maxGameInitializationAttemptsCount;
            }
            else
                this._gameInitializationAttemptsCountRest--;
        }

        this.gameStateChanged.next(this.gameState);

        return initializationResult;
    }

    moveSnake(): void {
        if (this.gameState == GameState.Started) {
            let followingSnakeHeadBrick: StandardBrick =
                this._brickOperationsExecutorService.move(this.field.snake.bricks.head, this.field.snake.direction, BaseGameLogicService._movementStep);

            if (this.field.bricks.isBrickCorrect(followingSnakeHeadBrick)) {
                switch (this.field.bricks.elements[followingSnakeHeadBrick.x][followingSnakeHeadBrick.y].type) {
                    case BrickType.Empty:
                        this.moveSnakeToEmptyBrick(followingSnakeHeadBrick);
                        break;
                    case BrickType.Food:
                        this.moveSnakeToFoodBrick(followingSnakeHeadBrick);
                        break;
                    default:
                        this.endGame();
                }
            }
            else
                this.endGame();
        }
    }

    pauseGame(): void {
        if (this.gameState == GameState.Started) {
            this._foodGeneratorTaskExecutor.pause();
            this._gameState = GameState.Paused;
            this.gameStateChanged.next(GameState.Paused);
        }
    }

    startGame(): void {
        if ((this.gameState != GameState.Finished) && (this.gameState != GameState.Started)) {
            if (this.gameState != GameState.NotStarted)
                this._foodGeneratorTaskExecutor.continue();

            this._gameState = GameState.Started;
            this.gameStateChanged.next(GameState.Started);
        }
    }

    foodEaten = new Subject<StandardBrick>();

    gameStateChanged = new Subject<GameState>();

    newFoodGenerated = new Subject<StandardBrick>();

    snakeDirectionChanged = new Subject<Direction>();

    snakePositionChanged = new Subject<Array<StandardBrick>>();

    snakeSpeedLevelChanged = new Subject<BonusType>();
}