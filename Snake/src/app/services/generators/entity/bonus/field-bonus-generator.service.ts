import { $enum } from "ts-enum-util";
import { BaseFieldBonusGeneratorService } from "./base-field-bonus-generator.service";
import { BaseFieldEntityGeneratorService } from "../base-field-entity-generator.service";
import { BonusType } from "src/app/enums/bonus-type";

export class FieldBonusGeneratorService extends BaseFieldBonusGeneratorService {

    generateBonus(): BonusType {
        return <BonusType>(BaseFieldEntityGeneratorService.parameterGenerator(0, $enum(BonusType).length));
    }
}