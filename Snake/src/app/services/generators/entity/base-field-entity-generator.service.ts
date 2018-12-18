import { MathService } from "../../math.service";

export abstract class BaseFieldEntityGeneratorService {

    protected static parameterGenerator = (minParameter: number, maxParameter: number) => MathService.getPseudoRandomNumber(minParameter, maxParameter);
}