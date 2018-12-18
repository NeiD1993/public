import { BaseFieldEntityGeneratorService } from "../base-field-entity-generator.service";
import { BonusType } from "src/app/enums/bonus-type";

export abstract class BaseFieldBonusGeneratorService extends BaseFieldEntityGeneratorService {

    abstract generateBonus(): BonusType;
}