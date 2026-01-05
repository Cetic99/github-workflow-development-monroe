/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import { useTranslation } from '@src/app/domain/administration/stores'
import IconRightArrow from '@ui/components/icons/IconRightArrow'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 2rem;
  line-height: 2.5rem;
  font-weight: 600;

  h4 {
    cursor: pointer;
  }

  div {
    display: flex;
    gap: 1rem;
    align-items: center;
    color: var(--secondary-dark);
  }
`

const SectionHeading = ({ title, to }) => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const handleNavigate = () => {
    navigate(to)
  }

  return (
    <Container>
      <h4>{t(title)}</h4>
      <div onClick={handleNavigate}>
        <h4>{t('See all')}</h4>
        <IconRightArrow color="#0D8472" size="m" />
      </div>
    </Container>
  )
}

export default SectionHeading
