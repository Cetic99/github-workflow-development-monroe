/* eslint-disable prettier/prettier */
/* eslint-disable no-case-declarations */

import { MessageType as ServerMessageType } from '@src/app/infrastructure/web-socket/index'
import { MessageType as ClientMessageType } from '@src/app/domain/operator/commands'
import { completeStep, failStep, getCardOperatorId, getStep } from '../stores/card-reader'
import { isEmpty, isNull, isUndefined } from 'lodash'
import { Mediator } from '@src/app/infrastructure/command-system'

const nextStep = (step, completed, nextStep) => {
  const storeStep = getStep(step)

  if (
    !isNull(storeStep) &&
    !isUndefined(storeStep) &&
    !isNull(storeStep?.success) &&
    !isUndefined(storeStep?.success)
  ) {
    //console.info('Step already completed or failed, ignoring...')
    return
  }

  if (!completed) {
    failStep(step)

    return
  }

  completeStep(step)

  const operatorId = getCardOperatorId()

  if (!isEmpty(nextStep)) {
    Mediator.dispatch(nextStep, {
      operatorId
    })
  }
}

export const SocketService = {
  process: (type, payload) => {
    switch (type) {
      case ServerMessageType.CardReaderInitialized:
        nextStep(ClientMessageType.InitializeCardReader, true, ClientMessageType.ScanUserCard)
        break
      case ServerMessageType.CardScanCompleted:
        nextStep(ClientMessageType.ScanUserCard, true, ClientMessageType.CreateIdentificationCard)
        break
      case ServerMessageType.CardEnrolled:
        nextStep(ClientMessageType.CreateIdentificationCard, true)
        break

      case ServerMessageType.CardReaderInitialzationFailed:
        nextStep(ClientMessageType.InitializeCardReader, false)
        break
      case ServerMessageType.CardScanFailed:
        nextStep(ClientMessageType.ScanUserCard, false)
        break
      case ServerMessageType.CardeEnrolmentFailed:
        nextStep(ClientMessageType.CreateIdentificationCard, false)
        break
      default:
        break
    }
  }
}
