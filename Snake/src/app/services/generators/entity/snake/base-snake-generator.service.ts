import { BaseFieldEntityGeneratorService } from "../base-field-entity-generator.service";
import { BaseFieldGeneratorService } from "../../field/base-field-generator.service";
import { BrickOperationsExecutorService } from "src/app/services/brick-operations-executor.service";
import { Bricks } from "src/app/models/bricks";
import { BrickType } from "src/app/enums/brick-type";
import { Direction } from "src/app/enums/direction";
import { FieldBrick } from "src/app/models/brick/field-brick";
import { Injectable } from "@angular/core";
import { SettingsOptionType } from "src/app/enums/settings-option-type";
import { SettingsService } from "src/app/services/settings.service";
import { Snake } from "src/app/models/snake/snake";
import { StandardBrick } from "src/app/models/brick/standard-brick";

@Injectable()
export abstract class BaseSnakeGeneratorService extends BaseFieldEntityGeneratorService {

    constructor(private _settingsService: SettingsService) {
        super();
    }

    private static generateSnakeBricks(headPosition: StandardBrick, length: number, snake: Snake, bricks: Bricks): void {
        let inverseSnakeDirection: Direction = snake.getInverseDirection();
        let snakeBrickIndex: number = length - 1;
        let snakeBrick: StandardBrick;

        while (snakeBrickIndex >= 0) {
            snakeBrick = BrickOperationsExecutorService.move(headPosition, inverseSnakeDirection, snakeBrickIndex);
            snake.bricks.enqueue(snakeBrick);
            BaseFieldGeneratorService.addBrick(bricks, new FieldBrick(snakeBrick.x, snakeBrick.y, BrickType.Snake));
            snakeBrickIndex--;
        }
    }

    protected abstract generateHeadPosition(bricks: Bricks): StandardBrick;

    protected abstract generateDirection(bricks: Bricks, headPosition: StandardBrick, length: number): Direction;

    initializeSnake(bricks: Bricks, snake: Snake): boolean {
        let headPosition: StandardBrick = this.generateHeadPosition(bricks);
        let length: number = this._settingsService.getSettingsOption(SettingsOptionType.SnakeInitialLength);
        let direction: Direction = this.generateDirection(bricks, headPosition, length);
        let inversedDirectoin: Direction = Snake.getInverseDirection(direction);

        if (direction !== undefined) {
            snake.direction = direction;
            snake.forbiddenDirection = inversedDirectoin;
            snake.bricks.clear();
            snake.setSpeedLevel(1);
            BaseSnakeGeneratorService.generateSnakeBricks(headPosition, length, snake, bricks);

            return true;
        }
        else
            return false;
    }
}