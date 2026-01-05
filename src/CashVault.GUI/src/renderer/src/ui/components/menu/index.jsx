import { useState, useRef } from 'react'
import styled from '@emotion/styled'
import ToggleInput from '@ui/components/inputs/toggle-input'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import Modal from '@ui/components/modal'

const Container = styled.div`
  background-color: var(--bg-medium);
  border-radius: 0.625rem;
  display: flex;
  flex-direction: column;
  height: 50vh;
  overflow: auto;

  & .menu-item-container {
    position: relative;
  }

  & .menu-item {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 1.875rem 2rem;

    h2 {
      display: inline-block;
      font-size: 1.5rem;
      color: #020605;
      font-weight: 500;
    }

    & .menu-item-button {
      display: flex;
      align-items: center;
      color: var(--secondary-dark);
      font-size: 1.5rem;
      font-weight: 600;
      cursor: pointer;
    }
  }

  & .separator {
    height: 1px;
    width: 100%;
    background-color: var(--border-dark);
    position: absolute;
    bottom: 0;
  }
`

const ModalContainer = styled.div`
  display: flex;
  flex-direction: column;

  & button {
    padding: 1rem;
    font-size: 1.5rem;
    background: none;
    border: none;
    text-align: left;
    cursor: pointer;
    &:hover {
      background-color: var(--bg-light);
    }
  }
`

const Menu = ({ options = [] }) => {
  const [selectedOptions, setSelectedOptions] = useState({})
  const [currentOption, setCurrentOption] = useState(null)
  const modalRef = useRef(null)

  const handleOpenModal = (item) => {
    setCurrentOption(item)
    modalRef.current?.showModal()
  }

  const handleSelectOption = (value) => {
    if (currentOption) {
      currentOption.onChange(value)
      setSelectedOptions((prev) => ({ ...prev, [currentOption.title]: value }))
      modalRef.current?.close()
    }
  }

  const handleCloseModal = () => {
    modalRef.current?.close()
  }

  const handleClickEvent = (item) => {
    item.onChange()
  }

  return (
    <>
      <Container>
        {options.map((item, index) => (
          <div className="menu-item-container" key={index}>
            <div className="menu-item">
              <h2>{item.title}</h2>
              {item.toggle && <ToggleInput onChange={(value) => item.onChange(value)} />}
              {!item.toggle && item.options && (
                <div className="menu-item-button" onClick={() => handleOpenModal(item)}>
                  {selectedOptions[item.title] || item.value}{' '}
                  <IconRightHalfArrow size="l" color="var(--secondary-dark)" />
                </div>
              )}
              {!item.toggle && !item.options && (
                <div className="menu-item-button" onClick={() => handleClickEvent(item)}>
                  {item.value} <IconRightHalfArrow size="l" color="var(--secondary-dark)" />
                </div>
              )}
            </div>
            <div className="separator"></div>
          </div>
        ))}
      </Container>

      <Modal ref={modalRef} hasClose={true} size="l" onClose={handleCloseModal}>
        <ModalContainer>
          {currentOption?.options &&
            currentOption?.options.map((opt, idx) => (
              <button key={idx} onClick={() => handleSelectOption(opt.value)}>
                {opt.title}
              </button>
            ))}
        </ModalContainer>
      </Modal>
    </>
  )
}

export default Menu
