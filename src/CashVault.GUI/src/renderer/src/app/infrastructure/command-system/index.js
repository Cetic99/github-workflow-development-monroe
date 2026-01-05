/* eslint-disable prettier/prettier */

export class Command {
  constructor(name, callback) {
    this.name = name
    this.callback = callback
  }

  execute(data) {
    this.callback?.(data)
  }
}

export class Mediator {
  static commands = []

  static clear() {
    this.commands = []
  }

  static register(command) {
    this.commands.push(command)
  }

  static registerMultiple(commands) {
    this.commands = [...this.commands, ...commands]
  }

  static dispatch(name, data) {
    let command = this.commands?.find((x) => x.name === name)

    command?.execute(data)
  }
}
