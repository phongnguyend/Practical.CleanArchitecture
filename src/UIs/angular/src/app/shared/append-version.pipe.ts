import { Pipe, PipeTransform, VERSION } from "@angular/core";

@Pipe({
  name: "appendVersion"
})
export class AppendVersionPipe implements PipeTransform {
  transform(value: string, prefix: string = " v"): string {
    return value + prefix + VERSION.full;
  }
}
