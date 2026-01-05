/* eslint-disable react/prop-types */
import { useRef, useState } from 'react'
import RetailModal from '@ui/components/retail-modal'
import styled from '@emotion/styled'
import Button from '../button'
import IconFilter from '../icons/IconFilter'
import IconCheckCircle from '../icons/IconCheckCircle'
import IconRepeat from '../icons/IconRepeat'
import IconBorderlessCheck from '../icons/IconBorderlessCheck'

const ModalContent = styled.div`
  & .modal-header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-bottom: 1rem;
    text-transform: capitalize;
  }

  white-space: pre-wrap;
  word-break: break-word;

  & .content-wrapper {
    overflow-y: auto;
    height: 50vh;
    padding-bottom: 1rem;
  }

  & .separator {
    width: 100%;
    height: 1px;
    background-color: #ccd3c7;
    margin-top: 2rem;
  }

  & .button-wrapper {
    display: flex;
    gap: 1.5rem;
    & .button {
      width: 13.375rem;
    }
  }

  & .gender {
    h2 {
      font-size: 2rem;
      font-weight: 600;
      line-height: 2.5rem;
      margin-bottom: 1.5rem;
    }
    margin-bottom: 4rem;
  }

  & .price-range {
    h2 {
      font-size: 2rem;
      font-weight: 600;
      line-height: 2.5rem;
      margin-bottom: 1.5rem;
    }

    & .button-wrapper {
      display: flex;
      flex-wrap: wrap;
      gap: 1.5rem;
    }
  }
  & .color {
    margin-top: 2rem;
    h2 {
      font-size: 2rem;
      font-weight: 600;
      line-height: 2.5rem;
      margin-bottom: 1.5rem;
    }

    & .button-wrapper {
      display: flex;
      flex-wrap: wrap;
      gap: 1.5rem;
    }
  }

  & .footer {
    margin-top: 2rem;
    display: flex;
    gap: 1.5rem;
    & .button {
      width: 21.25rem;
    }
  }
`

const ColorOption = styled.div`
  cursor: pointer;
  background-color: ${(p) => p.color};
  border: 1px solid #ccd3c7;
  width: 4rem;
  height: 4rem;
  border-radius: 100%;

  display: flex;
  justify-content: center;
  align-items: center;
  padding: 4px;

  box-sizing: border-box;
`

const priceRange = ['Under €50', '€50 - €100', '€100 - 150', '€150 - €200', 'Over €200']
const availableColors = ['#15c552', '#B6BBAA', '#FE9E59', '#FF5151']

const RetailFilter = ({ onApplyFilters = () => {} }) => {
  const [gender, setGender] = useState('')
  const [range, setRange] = useState('')
  const [color, setColor] = useState('')
  const filterModalRef = useRef()

  const handleModalClose = () => {
    filterModalRef?.current?.close()
  }

  const handleModalOpen = () => {
    filterModalRef?.current?.showModal()
  }

  const handleGenderChange = (value) => {
    setGender(value)
  }

  const handleRangeChange = (value) => {
    setRange(value)
  }

  const handleSelectedColor = (value) => {
    setColor(value)
  }

  const handleResetFilters = () => {
    setGender('')
    setRange('')
    setColor('')
  }

  const handleApplyFilters = () => {
    onApplyFilters({ gender, range, color })
    handleResetFilters()
    filterModalRef?.current?.close()
  }

  return (
    <>
      <Button onClick={handleModalOpen} icon={(props) => <IconFilter {...props} />}>
        Filter
      </Button>

      <RetailModal ref={filterModalRef} title="Filters" onClose={handleModalClose}>
        <ModalContent>
          <div className="content-wrapper">
            <div className="gender">
              <h2>Gender</h2>
              <div className="button-wrapper">
                <Button
                  rounded="s"
                  className="button"
                  color={gender === 'Men' ? 'secondary' : 'white'}
                  fontWeight={gender !== 'Men' ? 400 : 600}
                  onClick={() => handleGenderChange('Men')}
                >
                  Men
                </Button>
                <Button
                  rounded="s"
                  className="button"
                  color={gender === 'Women' ? 'secondary' : 'white'}
                  fontWeight={gender !== 'Women' ? 400 : 600}
                  onClick={() => handleGenderChange('Women')}
                  icon={null}
                >
                  Women
                </Button>
              </div>
            </div>
            <div className="price-range">
              <h2>Price range</h2>
              <div className="button-wrapper">
                {priceRange.map((rangeElement, index) => (
                  <Button
                    key={index}
                    rounded="s"
                    className="button"
                    color={rangeElement === range ? 'secondary' : 'white'}
                    fontWeight={rangeElement !== range ? 400 : 600}
                    onClick={() => handleRangeChange(rangeElement)}
                  >
                    {rangeElement}
                  </Button>
                ))}
              </div>
            </div>
            <div className="separator"></div>
            <div className="color">
              <h2>Color</h2>
              <div className="button-wrapper">
                {availableColors.map((colorElement, index) => (
                  <ColorOption
                    color={colorElement}
                    key={index}
                    chosen={colorElement === color}
                    onClick={() => handleSelectedColor(colorElement)}
                  >
                    {colorElement === color && <IconBorderlessCheck color="white" />}
                  </ColorOption>
                ))}
              </div>
            </div>
          </div>
          <div className="footer">
            <Button
              rounded="s"
              icon={(props) => <IconRepeat {...props} />}
              className="button"
              onClick={handleResetFilters}
              color="white"
            >
              Reset filters
            </Button>
            <Button
              rounded="s"
              className="button"
              color="dark"
              onClick={handleApplyFilters}
              icon={(props) => <IconCheckCircle {...props} />}
            >
              Apply
            </Button>
          </div>
        </ModalContent>
      </RetailModal>
    </>
  )
}

export default RetailFilter
