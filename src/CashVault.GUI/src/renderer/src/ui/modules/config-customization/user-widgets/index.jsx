/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import { useEffect, useRef, useState } from 'react'
import styled from '@emotion/styled'
import { createSwapy } from 'swapy'
import { useNavigate } from 'react-router-dom'

import { useUserWidgetsConfiguration } from '@domain/configuration/queries/user-widgets'
import { useTranslation } from '@domain/administration/stores'
import WidgetItem from './widget-item'
import { useUpdateUserWidgetsConfiguration } from '@domain/configuration/mutations/user-widgets'

import CircleButton from '@ui/components/circle-button'
import Button from '@ui/components/button'
import Modal from '@ui/components/modal'
import WidgetsContainer from '@ui/components/widgets'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import FullPageLoader from '@ui/components/full-page-loader'

const Container = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 1rem 2rem;
  overflow: hidden;
  padding-bottom: 1.5rem;
  margin-bottom: 7.5rem;

  & .actions {
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;

    display: flex;
    justify-content: space-between;
    align-items: center;

    padding: 1rem 3rem 1rem 2rem;
    z-index: 1000;

    .action-btn {
      button {
        box-shadow: var(--primary-dark) 0px 5px 15px;
      }
    }

    & > * {
      pointer-events: all;
    }
  }
`

const HeaderActions = styled.div`
  display: flex;
  flex-direction: row-reverse;
  padding: 1rem 0.25rem;
`

const ModalContent = styled.div`
  height: 100%;
  display: flex;
  flex-direction: column;

  & .header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-bottom: 1rem;
    flex-shrink: 0;
  }

  & .content {
    width: 100%;
    height: 100%;
  }

  & .footer {
    width: 100%;
    display: flex;
    justify-content: space-between;
    padding-top: 2rem;

    & .action-btn {
      width: 9rem;
      height: 4rem;
      display: flex;
      align-items: center;
      justify-content: center;
      text-align: center;
    }
  }
`

const UserWidgets = ({ data, onItemPropChange, saveChanges, onSwap }) => {
  const containerRef = useRef()
  const swapyRef = useRef()

  const { t } = useTranslation()

  const navigate = useNavigate()

  const handleOnGoBack = () => {
    navigate('/')
  }

  useEffect(() => {
    if (containerRef?.current) swapyRef.current = createSwapy(containerRef?.current)

    swapyRef?.current?.onSwap((data) => {
      onSwap(data?.draggingItem, data?.swappedWithItem)
    })

    return () => {
      swapyRef?.current?.destroy()
    }
  }, [])

  return (
    <Container ref={containerRef} id="slot-container">
      {data?.map((widget, index) => (
        <div key={`swapy-widget-${index}`} data-swapy-slot={widget.uuid}>
          <div data-swapy-item={widget.uuid}>
            <WidgetItem
              key={`widget-item-${index}`}
              {...widget}
              index={index}
              onPropChange={onItemPropChange}
            />
          </div>
        </div>
      ))}
      <div className="actions">
        <CircleButton
          size="l"
          textRight={t('Back')}
          icon={(props) => <IconLeftHalfArrow {...props} />}
          onClick={() => handleOnGoBack()}
          className="action-btn"
        />
        <CircleButton
          icon={(props) => <IconRightHalfArrow {...props} />}
          size="l"
          color="dark"
          textRight={t('Accept')}
          onClick={saveChanges}
          className="action-btn"
        />
      </div>
    </Container>
  )
}

const UserWidgetsScreen = () => {
  const { data: userWidgets, isLoading } = useUserWidgetsConfiguration()

  const [widgets, setWidgets] = useState(userWidgets)

  const previewModalRef = useRef()

  const { mutate: widgetsMutate } = useUpdateUserWidgetsConfiguration()

  const onSwap = (oldItemUuid, newItemUuid) => {
    const oldWidgetId = widgets?.findIndex((i) => i.uuid === oldItemUuid)
    const newWidgetId = widgets?.findIndex((i) => i.uuid === newItemUuid)

    if (oldWidgetId < 0 || !newWidgetId < 0) {
      return
    }

    const tempWidgets = [...widgets]

    const oldWidgetDisplaySequence = tempWidgets[oldWidgetId].displaySequence
    const newWidgetDisplaySequence = tempWidgets[newWidgetId].displaySequence

    tempWidgets[oldWidgetId].displaySequence = newWidgetDisplaySequence
    tempWidgets[newWidgetId].displaySequence = oldWidgetDisplaySequence

    setWidgets(tempWidgets)
  }

  const onItemPropChange = (uuid, index, propAccessor, value) => {
    const widget = widgets[index]

    if (!widget) {
      return
    }

    widget[propAccessor] = value

    setWidgets([...widgets.slice(0, index), widget, ...widgets.slice(index + 1)])
  }

  const saveChanges = () => {
    widgetsMutate({ widgets })
  }

  useEffect(() => {
    setWidgets(userWidgets)
  }, [userWidgets])

  if (!widgets || widgets.length < 1) {
    return <div>No data</div>
  }

  return (
    <>
      <FullPageLoader loading={isLoading} />
      <HeaderActions className="header-actions">
        <Button onClick={() => previewModalRef?.current?.showModal()}>{'Preview'}</Button>
      </HeaderActions>
      <UserWidgets
        data={widgets}
        onItemPropChange={onItemPropChange}
        saveChanges={saveChanges}
        onSwap={onSwap}
      />

      {/* Preview modal */}
      <Modal ref={previewModalRef} onClose={() => previewModalRef?.current?.close()}>
        <ModalContent>
          <div className="header">Widget preview</div>
          <div className="content">
            <WidgetsContainer
              widgets={widgets
                .filter((w) => w.enabled)
                .sort((a, b) => a.displaySequence - b.displaySequence)}
              isLoading={false}
            />
          </div>
        </ModalContent>
      </Modal>
    </>
  )
}

export default UserWidgetsScreen
