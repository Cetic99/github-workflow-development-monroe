/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import { useTranslation } from '@src/app/domain/administration/stores'
import IconSettings from '@ui/components/icons/IconSettings'
import ScreenContainerBottom from '@ui/layouts/screen-container-bottom'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  height: 100vh;
  z-index: 1000;
  position: absolute;
  overflow: auto;
  justify-content: center;

  padding: 0 11.25rem;
  h1 {
    font-size: 4.375rem;
    font-weight: 800;
    line-height: 4.375rem;
    margin: 1.5rem 0;
    color: white;
  }
  h2 {
    font-size: 1.625rem;
    color: var(--primary-light);
    font-weight: 400;
  }
  .cogwheel {
    width: fit-content;
    height: fit-content;
    animation: spin 8s linear infinite;
  }
  @keyframes spin {
    from {
      transform: rotate(0deg);
    }
    to {
      transform: rotate(360deg);
    }
  }
`

const FloatingLoading = ({ text }) => {
  const { t } = useTranslation()

  return (
    <ScreenContainerBottom>
      <Container>
        <div className="cogwheel">
          <IconSettings size="xl" color="var(--primary-light)" />
        </div>
        <h1>{t('One moment, please')}</h1>
        <h2>{text}</h2>
      </Container>
    </ScreenContainerBottom>
  )
}

export default FloatingLoading
