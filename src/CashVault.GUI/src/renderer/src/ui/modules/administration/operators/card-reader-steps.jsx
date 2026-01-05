/* eslint-disable prettier/prettier */

import styled from '@emotion/styled'

import { CardReaderSteps, CommandType, MessageType } from '@src/app/domain/operator/commands'
import {
  getFirstFailedStep,
  getStep,
  useActiveStep,
  useAllStepsCompleted,
  useAnyFailedSteps,
  useCardStoreActions
} from '@src/app/domain/administration/stores/card-reader'
import { useTranslation } from '@domain/administration/stores'
import { getOperatorId } from '@src/app/domain/global/stores'
import { Mediator } from '@src/app/infrastructure/command-system'

import IconBankCardView from '@ui/components/icons/IconBankCardView'
import IconCheckmark from '@ui/components/icons/IconCheckmark'
import IconCloseCircle from '@ui/components/icons/IconCloseCircle'
import IconATM from '@ui/components/icons/IconATM'
import IconBankCardAdd from '@ui/components/icons/IconBankCardAdd'
import IconBankCardCheck from '@ui/components/icons/IconBankCardCheck'
import Button from '@ui/components/button'
import { useEffect, useRef } from 'react'

const STEPS = [
  {
    key: MessageType.InitializeCardReader,
    description: '1. Wait for card reader initialization',
    renderIcon: (size, color) => <IconBankCardView size={size} color={color} />
  },
  {
    key: MessageType.ScanUserCard,
    description: '2. Put new card next to the card reader',
    renderIcon: (size, color) => <IconATM size={size} color={color} />
  },
  {
    key: MessageType.CreateIdentificationCard,
    description: '3. Adding card',
    renderIcon: (size, color) => <IconBankCardAdd size={size} color={color} />
  }
]

const VerticalDivider = styled.div`
  background-color: black;
  width: 0.125rem;
  height: 5.125rem;
`

const CardReaderStepsContent = () => {
  const { t } = useTranslation()

  const activeStepRef = useRef(null)

  const allStepsCompleted = useAllStepsCompleted()
  const activeStep = useActiveStep()

  const anyFailedSteps = useAnyFailedSteps()
  const failedStepIdx = getFirstFailedStep()

  const { reset: resetProcess } = useCardStoreActions()

  useEffect(() => {
    if (activeStepRef.current) {
      // scroll the active step into view with smooth behavior
      activeStepRef.current.scrollIntoView({
        behavior: 'smooth',
        block: 'nearest',
        inline: 'end'
      })
    }
  }, [activeStep])

  const reset = () => {
    const operatorId = getOperatorId()

    resetProcess()

    Mediator.dispatch(CommandType.InitializeCardReader, {
      id: operatorId
    })
  }

  const getColor = ({ active, success }) => {
    if (active || success === true) {
      return 'var(--primary-dark)'
    }

    return 'var(--text-medium)'
  }

  const getIconColor = (stepKey) => {
    const step = getStep(stepKey)

    return getColor({ active: step?.active, success: step?.success })
  }

  const renderStepContent = (step) => {
    const storeStep = getStep(step.key)

    // dont render step if not defined
    if (storeStep == null || storeStep == undefined) {
      return <></>
    }

    const stepIdx = CardReaderSteps.indexOf(step.key)

    // also, if one of previous steps failed, dont render next steps
    if (anyFailedSteps && stepIdx > failedStepIdx) {
      return <></>
    }

    const stepNotStarted = !allStepsCompleted && !anyFailedSteps && stepIdx > activeStep?.index

    // render failed step
    if (storeStep.success === false) {
      return (
        <>
          {step.renderIcon && step.renderIcon('xl', getIconColor(step.key))}
          <div className={`description ${stepNotStarted && 'disabled'}`}>
            {t(step.description)?.toUpperCase()}
          </div>
          <div className="status failed">{t('Failed')?.toUpperCase()}</div>
          <IconCloseCircle color={'var(--error-dark)'} size="m" />
          <Button className="retry-btn" onClick={reset}>
            {t('Retry')}
          </Button>
        </>
      )
    }

    if (storeStep.success === true || allStepsCompleted === true) {
      return (
        <>
          {step.renderIcon && step.renderIcon('xl', getIconColor(step.key))}
          <div className={`description ${stepNotStarted && 'disabled'}`}>
            {t(step.description)?.toUpperCase()}
          </div>
          <div className="status success">{t('Success')?.toUpperCase()}</div>
          <IconCheckmark color="white" />
          <VerticalDivider />
        </>
      )
    }

    if (activeStep?.key === step.key || storeStep.active === true) {
      return (
        <>
          {step.renderIcon && step.renderIcon('xl', getIconColor(step.key))}
          <div className={`description ${stepNotStarted && 'disabled'}`}>
            {t(step.description)?.toUpperCase()}
          </div>
          <div className="status success">{t('Processing')?.toUpperCase()}</div>
          <VerticalDivider />
        </>
      )
    }

    // step didn't started yet
    return (
      <>
        {step.renderIcon && step.renderIcon('xl', getIconColor(step.key))}
        <div className={`description disabled`}>{t(step.description)?.toUpperCase()}</div>
        <VerticalDivider />
      </>
    )
  }

  return (
    <>
      {STEPS.map((step, index) => (
        <div
          className={`step ${activeStep?.key === step.key && 'active'}`}
          key={`card-reader-modal-step__${index}`}
          ref={activeStep?.key === step.key ? activeStepRef : null}
        >
          {renderStepContent(step)}
        </div>
      ))}
      {/* Last step */}
      {anyFailedSteps === false && (
        <div
          className={`step ${allStepsCompleted && 'active'}`}
          ref={allStepsCompleted ? activeStepRef : null}
        >
          <IconBankCardCheck
            size="xl"
            color={getColor({
              success: allStepsCompleted
            })}
          />
          <div className={`description`}>
            {t('4. Use new card for operator login')?.toUpperCase()}
          </div>
          {allStepsCompleted && (
            <>
              <div className="status success">{t('Success')?.toUpperCase()}</div>
              <IconCheckmark color="white" />
            </>
          )}
        </div>
      )}
    </>
  )
}

export default CardReaderStepsContent
